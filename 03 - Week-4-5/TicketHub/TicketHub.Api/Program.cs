using Microsoft.EntityFrameworkCore;
using TicketHub.Data;
using TicketHub.Data.Entities;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() /* minimum lvl of logs to capture */
    .WriteTo.Console() /* Still having console logs */
    .WriteTo.File(
        path: "Logs/tickethub-log-.txt",
        rollingInterval: RollingInterval.Day, /* new file everyday automaticaly */
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();
/* Don't forget Log.CloseAndFlush(); at the end */

try
{
    Log.Information("Starting TicketHub API up...");

    var builder = WebApplication.CreateBuilder(args);

    /* Get Connection string from appsetings.json*/
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    /* Registry IDbContextFactory to manage concurrency safely */
    builder.Services.AddDbContextFactory<TicketHubDbContext>(options =>
        options.UseSqlServer(connectionString));

    /* Configuration of Swagger/OpenAPI to interact with the API */
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    /* This if-condition is to be secure other people with the access to the app
    can't see the swager avoiding attacks */
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }



    /* Test endpoint to validate the API is responding */
    app.MapGet("/ping", () => Results.Ok(new { message = "TicketHub API is live!" }));

    /* (CLEAR & RECREATE) First step before to seed DB */
    app.MapPost("/seed", async (IDbContextFactory<TicketHubDbContext> contextFactory,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Started seeding database");
        using var context = await contextFactory.CreateDbContextAsync();

        /* At time to destroy & recreate, EF Core reeds method OnModelCreating
        & insert data from .HasData() automátically. */
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        return Results.Ok(new { message = "Database reset successfully! Baseline data inserted via Model Seeds." });
    });

    /* Get all items from the ConcertSeats */
    app.MapGet("/inventory", async (IDbContextFactory<TicketHubDbContext> contextFactory,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Started ConcertSeats (Inventory) get from database");
        using var context = await contextFactory.CreateDbContextAsync();

        /* Consult seats charging (Include) its corresponding stock */
        var inventory = await context.ConcertSeats
            .Include(cs => cs.Stock)
            .Select(cs => new
            {
                cs.Id,
                cs.Sku,
                cs.Name,
                cs.Price,
                QuantityLeft = cs.Stock != null ? cs.Stock.QuantityOnHand : 0
            })
            .ToListAsync();

        return Results.Ok(inventory);
    });

    /* ******************************************************************************** */
    /* 1. Report: Top best-sell sections (total ticket volum) */
    app.MapGet("/reports/top-products", async (IDbContextFactory<TicketHubDbContext> contextFactory,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Started reports/top-products get from database");
        using var context = await contextFactory.CreateDbContextAsync();

        var topSeats = await context.BookingLines
            .Where(line => line.Booking.Status == BookingStatus.Fulfilled) /* only sum what was actually sold */
            .GroupBy(line => new { line.ConcertSeat.Sku, line.ConcertSeat.Name })
            .Select(group => new
            {
                group.Key.Sku,
                group.Key.Name,
                TotalTicketsSold = group.Sum(line => line.Quantity)
            })
            .OrderByDescending(x => x.TotalTicketsSold)
            .ToListAsync();

        return Results.Ok(topSeats);
    });

    /* 2. Report: Top Clients with most purchased tickets */
    app.MapGet("/reports/top-customers", async (IDbContextFactory<TicketHubDbContext> contextFactory,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Started reports/top-curstomers get from database");
        using var context = await contextFactory.CreateDbContextAsync();

        var topCustomers = await context.BookingLines
            .Where(line => line.Booking.Status == BookingStatus.Fulfilled)
            .GroupBy(line => new { line.Booking.Customer.Name, line.Booking.Customer.Email })
            .Select(group => new
            {
                group.Key.Name,
                group.Key.Email,
                TotalTicketsBought = group.Sum(line => line.Quantity)
            })
            .OrderByDescending(x => x.TotalTicketsBought)
            .ToListAsync();

        return Results.Ok(topCustomers);
    });

    /* 3. Report: Overall Fulfillment Rate (Fulfillment vs Backorder Rate) */
    app.MapGet("/reports/fulfillment-rate", async (IDbContextFactory<TicketHubDbContext> contextFactory,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Started reports/fulfillment-rate get from database");
        using var context = await contextFactory.CreateDbContextAsync();

        /* Total COuntof orders per each status */
        var totals = await context.Bookings
            .GroupBy(b => b.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToListAsync();

        int fulfilled = totals.FirstOrDefault(t => t.Status == BookingStatus.Fulfilled)?.Count ?? 0;
        int backordered = totals.FirstOrDefault(t => t.Status == BookingStatus.Backordered)?.Count ?? 0;
        int totalOrders = fulfilled + backordered;

        var report = new
        {
            TotalProcessedOrders = totalOrders,
            FulfilledCount = fulfilled,
            BackorderedCount = backordered,
            SuccessRatePercentage = totalOrders > 0 ? (double)fulfilled / totalOrders * 100 : 0
        };

        return Results.Ok(report);
    });
    /* ******************************************************************************** */

    /* auxiliar method process only one order in an atomic and secure maner managing concurrency */
    static async Task ProcessSingleOrderAsync(IDbContextFactory<TicketHubDbContext> contextFactory, 
    int customerId, int seatId, int quantityRequested)
    {
        /* each orden has its own exclusive short-term DbContext */
        using var context = await contextFactory.CreateDbContextAsync();

        /* Creating reservation header in Pendient state */
        var booking = new Booking
        {
            CustomerId = customerId,
            Priority = BookingPriority.Normal,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        /* Adding detailed line requesting tickets */
        var line = new BookingLine
        {
            BookingId = booking.Id,
            ConcertSeatId = seatId,
            Quantity = quantityRequested
        };
        context.BookingLines.Add(line);
        await context.SaveChangesAsync();

        int maxRetries = 3;
        int retryCount = 0;
        bool processed = false;

        while (!processed && retryCount < maxRetries)
        {
            /* Start a explicit transaction to assure the atomicity (all or nothing) */
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                /* 1. take the real inventory directly from the DB */
                var stock = await context.TicketStocks
                    .FirstOrDefaultAsync(ts => ts.ConcertSeatId == seatId);

                if (stock == null)
                {
                    booking.Status = BookingStatus.Rejected;
                    processed = true;
                }
                /* 2. Verify if there is alowd tickets */
                else if (stock.QuantityOnHand >= quantityRequested)
                {
                    /* Substract units from inventory */
                    stock.QuantityOnHand -= quantityRequested;

                    /* Update booking status */
                    booking.Status = BookingStatus.Fulfilled;
                    booking.CompletedAt = DateTime.UtcNow;

                    /* let a registry in the audit */
                    context.FulfillmentEvents.Add(new FulfillmentEvent
                    {
                        BookingId = booking.Id,
                        Type = "Success",
                        Message = $"Successfully secured {quantityRequested} tickets.",
                        Timestamp = DateTime.UtcNow
                    });

                    /* Try save changes. if other task modified the same stock
                    An exception ocurrs DbUpdateConcurrencyException by the RowVersion. */
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync(); /* Confirm the DB changes */
                    processed = true;

                    Console.WriteLine($"[INFO] Order {booking.Id} fulfilled successfully.");
                }
                else
                {
                    /* if ther isn't suficient stock, order runs a Backorder cleanly */
                    booking.Status = BookingStatus.Backordered;
                    booking.CompletedAt = DateTime.UtcNow;

                    context.FulfillmentEvents.Add(new FulfillmentEvent
                    {
                        BookingId = booking.Id,
                        Type = "Backorder",
                        Message = $"Insufficient stock. Requested {quantityRequested}, but only {stock.QuantityOnHand} left.",
                        Timestamp = DateTime.UtcNow
                    });

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    processed = true;

                    Console.WriteLine($"[WARN] Order {booking.Id} sent to Backorder due to low stock.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                /* ¡A COLLISION OCCURRED! another order modified the stock at the same time. */
                retryCount++;
                await transaction.RollbackAsync(); /* unDo the actual try */

                /* Forcing EFCore to reload the fresh data from the DB to the next try */
                context.ChangeTracker.Clear();

                Console.WriteLine($"[CONFLICT] Order {booking.Id} hit a conflict. Retrying ({retryCount}/{maxRetries})...");

                if (retryCount >= maxRetries)
                {
                    /* If it exhausted the retries, sent safely it to Backorder to avoid breaking the thread. */
                    booking.Status = BookingStatus.Backordered;
                    booking.CompletedAt = DateTime.UtcNow;

                    using var fallbackContext = await contextFactory.CreateDbContextAsync();
                    fallbackContext.Bookings.Update(booking);
                    await fallbackContext.SaveChangesAsync();

                    processed = true;
                }
            }
            catch (Exception ex)
            {
                /* To handling generic errors so that a damaged thread doesn't bring down the API */
                await transaction.RollbackAsync();
                Console.WriteLine($"[ERROR] Order {booking.Id} failed unexpectedly: {ex.Message}");
                processed = true;
            }
        }
    }

    /* Endpoint to simulate a massive burst of concurrent purchases */
    app.MapPost("/orders/burst", (IDbContextFactory<TicketHubDbContext> contextFactory, int n,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Started orders/burst database");
        /* Triggers the process in a background thread 
        (Task.Run) so that the endpoint responds immediately */
        _ = Task.Run(async () =>
        {
            Console.WriteLine($"[START] Starting a massive burst of {n} simulated ticket purchases...");

            var tasks = new Task[n];

            for (int i = 0; i < n; i++)
            {
                /* Simulation of alternating clients (Ids 1 to 4 from seed) */
                int customerId = (i % 4) + 1;

                /* Each client try to buy 2 tickets into section VIP (Id = 1, has 50 base units) */
                int seatId = 1;
                int quantityToBuy = 2;

                /* Save task in the array without wait yet */
                tasks[i] = ProcessSingleOrderAsync(contextFactory, customerId, seatId, quantityToBuy);
            }

            /* execute N tasks (simultaneous and concurrent) */
            await TaskExtensions.CombineAllOrExceptions(tasks); /* Use WhenAll on a safe way */

            Console.WriteLine($"[FINISHED] Burst of {n} orders processed completely.");
        });

        /* Immediately respond to see that the API is not frozen */
        return Results.Ok(new { message = $"Burst request for {n} orders accepted. Processing concurrently in the background." });
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application HTTP host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush(); /* Clean memory and assure writing of the file */
}

/* Quick extension to simulate WhenAll without losing individual exceptions */
public static class TaskExtensions
{
    public static async Task CombineAllOrExceptions(Task[] tasks)
    {
        try
        { await Task.WhenAll(tasks); }
        catch
        { /* Prevents an exception from abruptly halting the background process */ }
    }
}

