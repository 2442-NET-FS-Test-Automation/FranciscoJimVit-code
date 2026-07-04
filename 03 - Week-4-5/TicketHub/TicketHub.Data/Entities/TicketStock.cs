using System.ComponentModel.DataAnnotations;

namespace TicketHub.Data.Entities;

public class TicketStock
{
    public int Id { get; set; }

    public int ConcertSeatId { get; set; }

    /* Navegation relation to Seat/Section (1:1) */
    public ConcertSeat ConcertSeat { get; set; } = null!;

    public int QuantityOnHand { get; set; }

    /*  To avoid over-selling on masive concurrency */
    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}