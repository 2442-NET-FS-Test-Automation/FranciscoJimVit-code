using Microsoft.EntityFrameworkCore;
using Serilog;

using TicketHub.Data;
using TicketHub.Data.Entities;

namespace TicketHub.Data.Services;

public interface IFulfillmentService
{
    public Task FulfillBurstAsync(List<int> bookingIds, CancellationToken cancellationToken);
    public Task<string> FulfillOneAsync(int bookingId, CancellationToken ct);
}

public class FulfillmentService : IFulfillmentService
{
    private readonly IDbContextFactory<TicketHubDbContext> _contextFactory;

    public FulfillmentService(IDbContextFactory<TicketHubDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }


    public async Task FulfillBurstAsync(List<int> bookingIds, CancellationToken cancellationToken)
    {
        /* Executes ID processing in parallel, leveraging multiple background threads */
        await Parallel.ForEachAsync(bookingIds, cancellationToken, async (bookingId, token) =>
        {
            try
            {
                /* Delegate the stock reduction and logic into FulfillOneAsync */
                await FulfillOneAsync(bookingId, token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fulfill booking {BookingId} in burst mode", bookingId);
            }
        });
    }

    public async Task<string> FulfillOneAsync(int bookingId, CancellationToken ct)
    {
        /* Try loops limit */
        const int maxRetries = 10;
        int retryCount = 0;

        while (retryCount < maxRetries)
        {
            await using var db = await _contextFactory.CreateDbContextAsync(ct);

            var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, ct);
            if (booking == null) return "NotFound";

            /* Retrieves the seat including its TicketStock (where the RowVersion resides) */
            var seat = await db.ConcertSeats
                               .Include(cs => cs.Stock)
                               .FirstOrDefaultAsync(cs => cs.Stock != null && cs.Stock.QuantityOnHand > 0, ct);

            if (seat == null)
            {
                booking.Status = BookingStatus.Backordered;
                db.FulfillmentEvents.Add(new FulfillmentEvent
                {
                    BookingId = bookingId,
                    Type = "Backorder",
                    Message = "No seats available with sufficient stock",
                    Timestamp = DateTime.UtcNow
                });
                await db.SaveChangesAsync(ct);
                Log.Warning("Backordered {BookingId}: insufficient stock", bookingId);
                return "Backorder";
            }

            /* Stages entity tracking updates by appending booking lines */ 
            var detail = new BookingLine
            {
                BookingId = booking.Id,
                ConcertSeatId = seat.Id,
                Quantity = 1
            };
            db.BookingLines.Add(detail);

            /* Deducting stock, and recording the fulfillment event */
            seat.Stock!.QuantityOnHand -= 1;
            booking.Status = BookingStatus.Fulfilled;
            booking.CompletedAt = DateTime.UtcNow;

            db.FulfillmentEvents.Add(new FulfillmentEvent
            {
                BookingId = bookingId,
                Type = "Success",
                Message = $"Seat '{seat.Sku}' fulfilled by Thread {Environment.CurrentManagedThreadId}",
                Timestamp = DateTime.UtcNow
            });

            try
            {
                /* Try to save: If the row changed in the database */
                /* throws DbUpdateConcurrencyException if another thread updated RowVersion concurrently */
                await db.SaveChangesAsync(ct);

                Log.Information("Fulfilled booking: {BookingId} for seat {Sku} after {Retries} retries", bookingId, seat.Sku, retryCount);
                return "Success";
            }
            catch (DbUpdateConcurrencyException)
            {
                retryCount++;
                Log.Warning("Concurrency conflict detected for Booking {BookingId}. Retry attempt {Retry} of {MaxRetries}...", bookingId, retryCount, maxRetries);

                /* Waiting a few random milliseconds before retrying to mitigate thread collisions */
                await Task.Delay(Random.Shared.Next(10, 50), ct);
            }
        }

        
        /* If we exhaust all retries and the concurrency remains very high, we cancel the order due to conflict */
        await using var fallbackDb = await _contextFactory.CreateDbContextAsync(ct);
        var staleBooking = await fallbackDb.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, ct);
        if (staleBooking != null)
        {
            staleBooking.Status = BookingStatus.Backordered;
            fallbackDb.FulfillmentEvents.Add(new FulfillmentEvent
            {
                BookingId = bookingId,
                Type = "ConflictRetry",
                Message = "Order failed after exceeding maximum concurrency retries",
                Timestamp = DateTime.UtcNow
            });
            await fallbackDb.SaveChangesAsync(ct);
        }

        Log.Error("Backordered order {BookingId} after concurrency retry exhaustion", bookingId);
        return "ConflictRetry";
    }


}