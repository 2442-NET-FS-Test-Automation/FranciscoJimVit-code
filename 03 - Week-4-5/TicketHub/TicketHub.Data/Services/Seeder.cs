using Microsoft.EntityFrameworkCore;

using TicketHub.Data;
using TicketHub.Data.Entities;


namespace TicketHub.Data.Services;

public interface ISeeder
{
    List<int> SeedOrders(int n, bool expedited);
}


public class Seeder : ISeeder
{
    private readonly IDbContextFactory<TicketHubDbContext> _factory;

    public Seeder(IDbContextFactory<TicketHubDbContext> factory)
    {
        _factory = factory;
    }

    public List<int> SeedOrders(int n, bool expedited)
    {
        /* New clean DbContext */
        using var db = _factory.CreateDbContext();

        var generatedIds = new List<int>();

        for (int i = 0; i < n; i++)
        {
            /* Generation of a unique 8-character identifier to avoid duplicate emails */
            string uniqueId = Guid.NewGuid().ToString()[..8];

            var fakeCustomer = new Customer
            {
                Name = $"Burst Customer {uniqueId}",
                Email = $"burst_{uniqueId}@tickethub.com"
            };

            var newOrder = new Booking
            {
                Customer = fakeCustomer,
                Priority = expedited ? BookingPriority.Expedited : BookingPriority.Normal,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            db.Bookings.Add(newOrder);
            /* Persist saveing and get the ral ID of the BD */
            db.SaveChanges();

            generatedIds.Add(newOrder.Id);
        }

        return generatedIds;
    }

}