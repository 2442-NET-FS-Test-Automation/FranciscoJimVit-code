using Microsoft.EntityFrameworkCore;
using Serilog;

using TicketHub.Data;
using TicketHub.Data.Entities;
using TicketHub.Data.Services;

/* ******************************************************************************** */

/* ************************* Step 1: Configs and Services ************************* */
/* ********* Preparation: Tools and Dependencies before launching the app ********* */
var builder = WebApplication.CreateBuilder(args);
#region Step 1: Configuration and Services
    
    /* Get Connection string from appsetings.json*/
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    /* ******************************** Serilog Config ******************************** */
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console() /* Still having console logs */
        .WriteTo.File(
            path: "Logs/tickethub-log-.txt",
            rollingInterval: RollingInterval.Day, /* new file everyday automaticaly */
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        )
        .CreateLogger();
    /* Don't forget Log.CloseAndFlush(); at the end */
    builder.Host.UseSerilog(); /* Replace the native logger with Serilog: Lets do structred logs */
    /* ******************************************************************************** */

    Log.Information("Starting TicketHub API up...");
    

    /* Registry IDbContextFactory to manage concurrency safely */
    builder.Services.AddDbContextFactory<TicketHubDbContext>(options => options.UseSqlServer(connectionString));
    
    builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
    
    builder.Services.AddScoped<ISeeder, Seeder>();
    builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();


    /* Configuration of Swagger/OpenAPI to interact with the API */
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

#endregion

/* ************************* Step 2: Execution and Logic ************************* */
/* ********** Runing: App methods, Define Endpoints, and procesing data ********** */
var app = builder.Build();
#region Step 2: Execution and Logics
    
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
        using var db = await contextFactory.CreateDbContextAsync();

        /* At time to destroy & recreate, EF Core reeds method OnModelCreating
        & insert data from .HasData() automátically. */
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        return Results.Ok(new { message = "Database reset successfully! Baseline data inserted via Model Seeds." });
    });

    /* Get all items from the ConcertSeats */
    app.MapGet("/inventory", async (IDbContextFactory<TicketHubDbContext> contextFactory,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Started ConcertSeats (Inventory) get from database");
        using var db = await contextFactory.CreateDbContextAsync();

        /* Consult seats charging (Include) its corresponding stock */
        var inventory = await db.ConcertSeats
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
        using var db = await contextFactory.CreateDbContextAsync();

        var topSeats = await db.BookingLines
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
        using var db = await contextFactory.CreateDbContextAsync();

        var topCustomers = await db.BookingLines
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
        using var db = await contextFactory.CreateDbContextAsync();

        /* Total COuntof orders per each status */
        var totals = await db.Bookings
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


    /* Endpoint to simulate a massive burst of concurrent purchases */
    app.MapPost("/orders/burst", (int n, bool expedited, ISeeder seeder,
        IServiceScopeFactory scopes, IHostApplicationLifetime lifetime) =>
    {
        var ids = seeder.SeedOrders(n, expedited);
        var appStopping = lifetime.ApplicationStopping;

        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = scopes.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IFulfillmentService>();
                
                /* Parallel procesing IDs into Fulfillment Service */
                await service.FulfillBurstAsync(ids, appStopping);
            } 
            catch (Exception ex)
            {
                Log.Error(ex, "Burst fulfillment failed");
            }
        }, appStopping);

        return Results.Accepted(value: new { message = $"Burst request for {n} orders accepted. Processing concurrently in the background." });
    });

#endregion

/* ************************* Step 3: Application Liftime ************************* */
/* ********* Launching: Run and Stop point, prepare exit clear with logs ********* */
app.Run();
#region Step 3: Application Lifetime

    Log.Information("TicketHub API is shutting down...");
    Log.CloseAndFlush(); /* Clean memory and assure writing of the file */

#endregion

