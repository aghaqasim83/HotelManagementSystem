using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.SeedManager;

public static class DI
{
    public static IServiceCollection RegisterDemoSeedManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<HotelSeeder>();

        // register the application-level seeder abstraction
        services.AddScoped<IDbSeeder, DbSeeder>();
        services.AddScoped<IDbMaintenance, DbMaintenance>();

        return services;
    }
}
