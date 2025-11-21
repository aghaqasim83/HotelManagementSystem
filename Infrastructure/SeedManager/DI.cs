using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.SeedManager;

public static class DI
{
    public static IServiceCollection RegisterDemoSeedManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<HotelSeeder>();

        return services;
    }

    public static IHost SeedDemoData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var context = serviceProvider.GetRequiredService<DataContext>();
        if (!context.Hotel.Any()) //if empty, thats mean never been seeded before
        {
            var customerCategorySeeder = serviceProvider.GetRequiredService<HotelSeeder>();
            customerCategorySeeder.GenerateDataAsync().Wait();
        }

        return host;
    }
}
