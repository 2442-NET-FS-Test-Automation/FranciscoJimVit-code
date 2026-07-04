# Solution + the two projects (Minimal API host, EF Core class library)
dotnet new sln -n TicketHub
dotnet new web -n TicketHub.Api
dotnet new classlib -n TicketHub.Data
## Connect projects eachother
dotnet sln add TicketHub.Api TicketHub.Data
dotnet add TicketHub.Api reference TicketHub.Data


# Packages for Data Layout (TicketHub.Data)
dotnet add TicketHub.Data package Microsoft.EntityFrameworkCore.SqlServer
dotnet add TicketHub.Data package Microsoft.EntityFrameworkCore.Design
# Packages for API host Layout (TicketHub.Api)
dotnet add TicketHub.Api package Microsoft.EntityFrameworkCore.Design
dotnet add TicketHub.Api package Serilog.AspNetCore
dotnet add TicketHub.Api package Swashbuckle.AspNetCore


