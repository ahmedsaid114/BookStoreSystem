using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore535.API.Utilities.DbInitializer;

public sealed class DbInitializer(
    RoleManager<IdentityRole<int>> roles,
    UserManager<CustomerAccount> users,
    BookStore535.API.Data.ApplicationDbContext context,
    IConfiguration configuration) : IDbInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        foreach (var roleName in AppRoles.All)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await roles.RoleExistsAsync(roleName)) continue;

            var result = await roles.CreateAsync(new IdentityRole<int>(roleName));
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Could not create role '{roleName}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
        }

        await EnsureAdminAsync(cancellationToken);

        var categories = new[] { "Business", "Self Help", "History", "Romance", "Fantasy", "Art", "Kids", "Music", "Cooking" };
        foreach (var categoryName in categories)
        {
            if (await context.Categories.AnyAsync(category => category.Name == categoryName, cancellationToken)) continue;
            context.Categories.Add(new Category { Name = categoryName, IsActive = true });
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAdminAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var email = configuration["Admin:Email"]?.Trim();
        var password = configuration["Admin:Password"];
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password)) return;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Set both Admin:Email and Admin:Password in User Secrets before starting the API.");

        var admin = await users.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new CustomerAccount
            {
                Name = "Book Store Admin",
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                CreatedUtc = DateTime.UtcNow
            };
            var created = await users.CreateAsync(admin, password);
            if (!created.Succeeded)
                throw new InvalidOperationException($"Could not create admin account: {string.Join("; ", created.Errors.Select(error => error.Description))}");
        }

        if (!await users.IsInRoleAsync(admin, AppRoles.Admin))
        {
            var assigned = await users.AddToRoleAsync(admin, AppRoles.Admin);
            if (!assigned.Succeeded)
                throw new InvalidOperationException($"Could not assign Admin role: {string.Join("; ", assigned.Errors.Select(error => error.Description))}");
        }
    }
}

