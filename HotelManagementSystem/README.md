# HotelManagementSystem

Summary
-------
HotelManagementSystem is a .NET 10 web API implementing a simple hotel/room/booking domain with a layered architecture:
- Domain: domain entities and shared domain types.
- Application: CQRS-style features (MediatR) — commands, queries, validators and DTOs.
- Infrastructure: data access (EF Core), repositories/unit-of-work, seeders and other infra services.
- HotelManagementSystem (web project): ASP.NET Core Web API host, controllers, middleware and wiring.

Key technologies
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core (EF Core) with SQL Server provider
- MediatR for command/query handlers
- FluentValidation for request validation
- Swashbuckle (Swagger / OpenAPI) for interactive API docs
- Serilog (light usage seen in DI)
- Pattern: generic ICommandRepository<T>, UnitOfWork, extension methods to register infrastructure

Project structure (high level)
- Domain/
  - Entities: `Hotel`, `Room`, `Booking`, base entity types and enums.
- Application/
  - Common: repository interfaces (`ICommandRepository<>`, `IEntityDbSet`, etc.), CQRS contracts.
  - Features:
    - BookingManager: `CreateBooking` command, validators, handlers and queries (FindByReference).
    - HotelManager: `GetHotelByName`, `GetAvailableRooms`, validators, handlers.
- Infrastructure/
  - DataAccessManager/EFCore: DbContexts, EF configurations, repository implementations, DI helper.
  - SeedManager: seeders (e.g., `HotelSeeder`) and DI helpers to register seed services.
  - DependencyInjection entrypoint to register all infra services.
- HotelManagementSystem/ (web)
  - `Program.cs` — host building and middleware.
  - `BackEndConfiguration.cs` — application service wiring (calls `AddInfrastructureServices`).
  - Controllers: `BookingController`, `HotelController`, `WeatherForecastController` (template).
  - Middlewares: global exception handler wrapper.

How it works
- Requests routed to controllers (e.g., `api/booking`, `api/hotel`).
- Controllers forward requests to MediatR (`ISender`) which invokes command/query handlers.
- Handlers use `ICommandRepository<T>.GetQuery()` and EF Core to query/store data and use `IUnitOfWork` to commit.
- Infrastructure registers EF Core DbContext(s) and repository/unit-of-work implementations through extension methods.- 

**Important endpoints (examples)**
- Find hotel by name
  - GET /api/hotel/FindByName?name=Premier%20Inn
- Find available rooms
  - GET /api/hotel/AvailableRooms?hotelId={hotelId}&checkIn=2025-12-01&checkOut=2025-12-03&people=2
  - Date format for query parameters: yyyy-MM-dd (DateOnly binding)
- Create booking
  - POST /api/booking
  - Body: CreateBookingRequest { RoomId, GuestName, CheckIn, CheckOut, NumberOfGuests }
- Find booking by reference
  - GET /api/booking/FindByReference?reference={bookingReference}
- Clear database (development only)
  -	POST /api/DatabaseManager/Clear
  -	Response: ApiSuccessResult<ClearDatabaseResult> (content: ClearDatabaseResult { bool Success; string? Message })
- Seed database (development only)
  - POST /api/DatabaseManager/Seed?seedDemo={true|false}
  -	Query: seedDemo (optional, boolean) — default true (when true runs demo seeding)
  -	Response: ApiSuccessResult<SeedDatabaseResult> (content: SeedDatabaseResult { bool Success; string? Message })

Swagger / OpenAPI
- The project includes Swashbuckle. In Development the Swagger UI is enabled.
- Start the app and open: `https://localhost:{port}/swagger/index.html` (or `/swagger`).
- If UI fails to fetch JSON, verify the app is running and use the same scheme and port for the Swagger endpoint (see application logs).

Configuration and connection string
- appsettings.json contains `ConnectionStrings:DefaultConnection` and `DatabaseProvider`.
- Example Windows Authentication connection string:
  - `Server=localhost\SQLEXPRESS;Database=WHMS-LTE-FS;Trusted_Connection=True;TrustServerCertificate=True;`
- Ensure the web project can reach the database and the running Windows account has DB permissions when using integrated security.

DI / Common runtime issues
- If `UseSqlServer` or other provider extension methods are missing, add the provider package:
  - `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
- If `AddSwaggerGen` or `UseSwagger` not found, add:
  - `dotnet add package Swashbuckle.AspNetCore`
- Ensure the Infrastructure project is referenced by the web project so extension methods (CreateDatabase, SeedDemoData, repository registrations) are available.
- The seeders depend on `ICommandRepository<T>` and `IUnitOfWork` — ensure those registrations are present in `Infrastructure.DataAccessManager.EFCore.DI`.

Development tips
- Run `dotnet restore` and `dotnet build` in solution root.
- To trust local HTTPS dev cert: `dotnet dev-certs https --trust` (if browser blocks cert).
- Watch the application logs (Output window) for lines like: `Now listening on: https://localhost:7227` — use that URL/port for testing.
- The project uses `DbContext.Database.EnsureCreated()` for development seeding; consider migrations for production.

Testing and extensibility
- Handlers and repositories are designed for unit testing — mock `ICommandRepository<T>` and `IUnitOfWork`.
- Add more queries/commands in `Application.Features.*` following existing patterns.
- To change seeding behavior, review `Infrastructure/SeedManager` and the calls in `BackEndConfiguration.RegisterBackEndBuilder`.

Contact / next steps
- To run a quick test:
  1. Ensure DB connection (or switch to local SQL or use Docker SQL container).
  2. Start the web project (F5 / Ctrl+F5).
  3. Call endpoints via Postman or open Swagger UI.
- If you want, I can:
  - Add OpenAPI examples for each endpoint.
  - Add integration test templates for booking flow.
  - Convert EnsureCreated to EF Migrations.
