using System.ComponentModel.DataAnnotations;

namespace TicketHub.Data.Entities;

/* The audit log. It records which thread process the order and its final result. */

public class FulfillmentEvent
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!; /* Example: "Success", "ConflictRetry", "Backorder" */

    [Required, StringLength(50)]
    public string Type { get; set; } = null!;

    [Required, StringLength(500)]
    public string Message { get; set; } = null!;

    public DateTime TimeStamp { get; set; }
}