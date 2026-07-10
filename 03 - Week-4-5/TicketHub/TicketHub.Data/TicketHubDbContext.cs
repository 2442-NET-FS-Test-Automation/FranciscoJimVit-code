using Microsoft.EntityFrameworkCore;
using TicketHub.Data.Entities;

namespace TicketHub.Data;

/* : DbContext is the Father class from EntityFramework */
public class TicketHubDbContext : DbContext
{   
    /* DbContextOptions is the object that contains all DB configurations 
    Example: DB engine, connection string, behavios etc */
    public TicketHubDbContext(DbContextOptions<TicketHubDbContext> options) : base(options) {}

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ConcertSeat> ConcertSeats => Set<ConcertSeat>();
    public DbSet<TicketStock> TicketStocks => Set<TicketStock>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingLine> BookingLines => Set<BookingLine>();
    public DbSet<FulfillmentEvent> FulfillmentEvents => Set<FulfillmentEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder);

        /* Customer Configuration */
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(c => c.Email).IsUnique();  /* Integrity by Unique index at Email */
        });

        /* ConcertSeat Configuration */
        modelBuilder.Entity<ConcertSeat>(entity =>
        {
            entity.HasIndex(s => s.Sku).IsUnique(); 
            
            entity.Property(s => s.Price).HasColumnType("decimal(10,2)");
        });

        /* TicketStock Configuration - Relation(1:1) with ConcertSeat */
        modelBuilder.Entity<TicketStock>(entity =>
        {
            entity.HasOne(ts => ts.ConcertSeat)
                .WithOne(cs => cs.Stock)
                .HasForeignKey<TicketStock>(ts => ts.ConcertSeatId)
                .OnDelete(DeleteBehavior.Cascade);

            /* Concurrency Token */
            entity.Property(ts => ts.RowVersion)
                .IsRowVersion();
        });

        /* Booking Configurations */
        modelBuilder.Entity<Booking>(entity =>
        {   
            /* Booking has enums for Priority and Status so needs Parse to String */
            entity.Property(b => b.Priority).HasConversion<string>();
            entity.Property(b => b.Status).HasConversion<string>();

            /* Non-Key Index */
            entity.HasIndex(b => b.Status);
        });


        /* ******************************************************************* */
        /*          After configurate entities, is necesary Seed the BD        */
        /* ******************************************************************* */

        /* 1. Clear & recreate BD, (Reset endpoint that restores baseline). */
        /* Deployed step on Program.cs with app.MapPost("/seed" */

        /* 2. Seed Test Clients */
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Alice Vance", Email = "alice@tickets.com" },
            new Customer { Id = 2, Name = "Bob Miller", Email = "bob@tickets.com" },
            new Customer { Id = 3, Name = "Charlie Smith", Email = "charlie@tickets.com" },
            new Customer { Id = 4, Name = "Diana Prince", Email = "diana@tickets.com" }
        );

        /* 3. Seed of Seats/Sections (ConcertSeats) */
        /* NOTE: Not assigned 'Stock' navigatiom property here, It'll be seeded separately */
        modelBuilder.Entity<ConcertSeat>().HasData(
            new ConcertSeat { Id = 1, Sku = "VIP-ZONE", Name = "VIP Front Stage", Price = 250.00m },
            new ConcertSeat { Id = 2, Sku = "GEN-FLOOR", Name = "General Admission Standing", Price = 85.00m },
            new ConcertSeat { Id = 3, Sku = "BALCONY-A", Name = "Balcony Preferent A", Price = 120.00m }
        );

        /* 4. Seed of inventory (TicketStocks) */
        /* Links each inventory with it's ConcertSeat using property ConcertSeatId */
        modelBuilder.Entity<TicketStock>().HasData(
            new TicketStock { Id = 1, ConcertSeatId = 1, QuantityOnHand = 50 },
            new TicketStock { Id = 2, ConcertSeatId = 2, QuantityOnHand = 500 },
            new TicketStock { Id = 3, ConcertSeatId = 3, QuantityOnHand = 150 }
        );
    }


}