# BookStoreSystem

Standalone bookstore backend Web API built with ASP.NET Core, Entity Framework Core and SQL Server. No UI - API only, documented with OpenAPI and Scalar.

For a guided Arabic walkthrough of the API, see [API_GUIDE_AR.md](BookStore535.API/API_GUIDE_AR.md).

## Features

- Book catalog with categories, authors and publishers
- Book reviews and ratings
- Shopping cart and wishlist
- Discount codes and book orders
- ASP.NET Core Identity with Customer, Admin and SuperAdmin roles
- DTOs with Mapster mapping, custom validation attributes
- Generic repository pattern
- API documentation with OpenAPI and Scalar

## Tech stack

- .NET 10, ASP.NET Core Web API
- Entity Framework Core (code-first migrations)
- Microsoft SQL Server
- ASP.NET Core Identity
- Mapster, Scalar (OpenAPI)

## Getting started

1. Install the .NET 10 SDK.
2. Set `ConnectionStrings:DefaultConnection` in `BookStore535.API/appsettings.json` to your SQL Server instance.
3. Apply the database migrations:

   ```powershell
   dotnet ef database update
   ```

4. Run the project:

   ```powershell
   dotnet run --project BookStore535.API
   ```

5. Open the Scalar API docs at `/scalar` to explore and test the endpoints.
