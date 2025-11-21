using Infrastructure.DataAccessManager.EFCore;
using Infrastructure.SeedManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //>>> DataAccess
        services.RegisterDataAccess(configuration);

        services.RegisterDemoSeedManager(configuration);

        return services;
    }
}
