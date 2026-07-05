using Microsoft.EntityFrameworkCore;
using TicketHub.Data;

var builder = WebApplication.CreateBuilder(args);

/* Get Connection string from appsetings.json*/
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

/* Registry IDbContextFactory to manage concurrency safely */
builder.Services.AddDbContextFactory<TicketHubDbContext>(options => 
    options.UseSqlServer(connectionString));

/* Configuration of Swagger/OpenAPI to interact with the API */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/* This if-condition is to be secure other people with the access to the app
can't see the swager avoiding attacks */
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* Test endpoint to validate the API is responding */
app.MapGet("/ping", () => Results.Ok(new { message = "TicketHub API is live!" }));

app.Run();
