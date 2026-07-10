using System.ComponentModel.DataAnnotations;

namespace TicketHub.Data.Entities;

/* Customer represent a Fanatic who mades the purchase.
Email shall be unique (It will be forced with Fluent API). */

public class Customer
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [Required, StringLength(150)]
    public string Email { get; set; } = null!;
}