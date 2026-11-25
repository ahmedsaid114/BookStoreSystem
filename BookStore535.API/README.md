# BookStore535 API

Standalone ASP.NET Core Web API backend for the linked bookstore storefront. The frontend is only a read-only reference; this API is not wired to or deployed with that site. The project has no Views or Razor pages.

The structure follows the supplied `Ecommerce535.API` example: `Areas`, `CustomValidations`, `DTOs/Request`, `DTOs/Response`, `Data`, `Migrations`, `Models`, `Repositories`, `Utilities`, `ApplicationConfiguration.cs`, and `globalUsings.cs`.

## Run locally

1. Install .NET 10 SDK and SQL Server.
2. Update `ConnectionStrings:DefaultConnection` in `appsettings.json`.
3. Set an administrator account in User Secrets before running admin operations:

   ```powershell
   dotnet user-secrets set "Admin:Email" "admin@example.com"
   dotnet user-secrets set "Admin:Password" "Use-A-Strong-Password-123"
   ```

4. Run `dotnet run`. EF Core applies migrations at startup and the initializer seeds store categories and Identity roles.
5. Open `/scalar` for the API reference.

The checkout defaults to free shipping and 0% tax in `appsettings.json`; configure these values for the intended market. Checkout currently records Cash on Delivery orders and does not call an external payment provider.

See [API_GUIDE_AR.md](API_GUIDE_AR.md) for the endpoint map, request bodies, and database model.
