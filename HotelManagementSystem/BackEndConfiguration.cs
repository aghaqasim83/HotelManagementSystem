namespace HotelManagementSystem;

using Application;
using HotelManagementSystem.Common.Handlers;
using Infrastructure;
using Infrastructure.DataAccessManager.EFCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

public static class BackEndConfiguration
{
    public static IServiceCollection AddBackEndServices(this IServiceCollection services, IConfiguration configuration)
    {
        //>>> Application Layer
        services.AddApplicationServices();

        //>>> Infrastructure Layer
        services.AddInfrastructureServices(configuration);

        services.AddExceptionHandler<CustomExceptionHandler>();

        //>>> Common

        services.AddHttpContextAccessor();
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Indotalent API", Version = "v1" });
        });


        services.Configure<ApiBehaviorOptions>(x =>
        {
            x.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }

    public static IApplicationBuilder RegisterBackEndBuilder(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        IHost host,
        IConfiguration configuration
        )
    {
        // >>> Create database
        host.CreateDatabase();

        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Indotalent V1");
            });
        }

        return app;
    }

}