using Microsoft.EntityFrameworkCore;
using TicketHub.Data.Entities;

namespace TicketHub.Data;

/* : DbContext is the Father class from EntityFramework */
public class TicketHubDbContext : DbContext
{   
    /* DbContextOptions is the object that contains all DB configurations 
    Example: DB engine, connection string, behavios etc */
    public TicketHubDbContext(DbContextOptions<TicketHubDbContext> options) : base(options) {}

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<ConcertSeat> ConcertSeats { get; set; } = null!;
    public DbSet<TicketStock> TicketStocks { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<BookingLine> BookingLines { get; set; } = null!;
    public DbSet<FulfillmentEvent> FulfillmentEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }


}