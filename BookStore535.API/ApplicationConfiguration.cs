using BookStore535.API.Repositories;
using BookStore535.API.Utilities.DbInitializer;

namespace BookStore535.API;

public static class ApplicationConfiguration
{
    public static IServiceCollection RegisterConfig(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDbInitializer, DbInitializer>();
        return services;
    }
}

