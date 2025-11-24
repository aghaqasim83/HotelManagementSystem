using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.DatabaseManager;

public static class HostExtensions
{
    /// <summary>
    /// Calls the registered IDbSeeder to populate demo data. Safe to call from startup.
    /// </summary>
    public static IHost SeedDemoData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetService<IDbSeeder>();
        if (seeder != null)
        {
            // keep same synchronous API as before
            seeder.SeedDemoDataAsync().GetAwaiter().GetResult();
        }

        return host;
    }
}