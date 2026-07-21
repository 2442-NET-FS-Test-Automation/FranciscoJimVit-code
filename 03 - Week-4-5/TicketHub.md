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

## - - - - - - - - - - - - - 
# To create a service in docker Only for new DB services: 
## I'm not use this command because I reuse the Library server
docker run -d --name librarysqlserver -p 1433:1433 \ -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD='libraryPass1!' \ mcr.microsoft.com/mssql/server:2022-latest

# To start Docker - Directly on Docker Desktop
docker start librarysqlserver
## - - - - - - - - - - - - - 

# First Migration
## 1. Crear el diseño de la migración inicial leyendo las entidades
dotnet ef migrations add InitialCreate --project TicketHub.Data --startup-project TicketHub.Api
## 2. Aplicar el diseño y crear físicamente las tablas en SQL Server
dotnet ef database update --project TicketHub.Data --startup-project TicketHub.Api