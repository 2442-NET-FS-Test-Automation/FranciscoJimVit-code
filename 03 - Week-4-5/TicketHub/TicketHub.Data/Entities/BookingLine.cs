namespace TicketHub.Data.Entities;

/* Is the booking details (An "OrderLine").
It links how many tickets were requested for which section (ConcertSeat). */

public class BookingLine
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public int ConcertSeatId { get; set; }
    public ConcertSeat ConcertSeat { get; set; } = null!;

    public int Quantity { get; set; }
}