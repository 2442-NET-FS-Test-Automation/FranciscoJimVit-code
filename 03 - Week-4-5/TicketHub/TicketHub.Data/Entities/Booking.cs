using System.ComponentModel.DataAnnotations;

namespace TicketHub.Data.Entities;

/* Booking represents purchased transaction of tickets (An "Order").
It includes enums to control priorities (Expedited or Normal) and 
Process State (Pending, Fulfilled, Backordered, Rejected). */

public enum BookingPriority { Normal, Expedited }
public enum BookingStatus { Pending, Fulfilled, Backordered, Rejected }

public class Booking
{
    public int Id { get; set; }
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public BookingPriority Priority { get; set; }
    public BookingStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    /* Relation one to many with their detail lines */
    public List<BookingLine> Lines { get; set; } = new();
}