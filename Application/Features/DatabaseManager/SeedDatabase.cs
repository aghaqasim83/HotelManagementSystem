using Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.DatabaseManager;

public class SeedDatabaseRequest : IRequest<SeedDatabaseResult>
{
    /// <summary>
    /// When true, seed demo data. Defaults to true.
    /// </summary>
    public bool SeedData { get; set; } = true;
}

public class SeedDatabaseResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class SeedDatabaseHandler : IRequestHandler<SeedDatabaseRequest, SeedDatabaseResult>
{
    private readonly IDbSeeder _dbSeeder;
    private readonly ILogger<ClearDatabase> _logger;

    public SeedDatabaseHandler(IDbSeeder dbSeeder, ILogger<ClearDatabase> logger)
    {
        _dbSeeder = dbSeeder;
        _logger = logger;
    }

    public async Task<SeedDatabaseResult> Handle(SeedDatabaseRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.SeedData)
            {
                await _dbSeeder.SeedDemoDataAsync();
            }

            return new SeedDatabaseResult { Success = true, Message = "Seeding completed." };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed database.");
            return new SeedDatabaseResult { Success = false, Message = ex.Message };
        }
    }
}