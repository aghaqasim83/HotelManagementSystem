using HotelManagementSystem;
using HotelManagementSystem.Common.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddBackEndServices(builder.Configuration);

var app = builder.Build();

app.RegisterBackEndBuilder(app.Environment, app, builder.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    //http://localhost:5191/swagger/index.html
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        // c.RoutePrefix = string.Empty; // serve UI at root if you prefer
    });

}
app.UseCors();
app.UseMiddleware<GlobalApiExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
