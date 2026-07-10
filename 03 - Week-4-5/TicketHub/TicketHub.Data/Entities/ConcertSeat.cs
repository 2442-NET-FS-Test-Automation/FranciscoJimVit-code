using System.ComponentModel.DataAnnotations;

namespace TicketHub.Data.Entities;

public class ConcertSeat
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Sku { get; set; } = null!; /* Example: "VIP-ZONE-01" */
    
    [Required, StringLength(100)]
    public string Name { get; set; } = null!; /* Example: "Numered VIP Zone" */
    
    public decimal Price { get; set; }
    
    public TicketStock? Stock { get; set; }

}