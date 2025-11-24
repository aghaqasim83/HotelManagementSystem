using Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.DatabaseManager;

public class ClearDatabaseRequest : IRequest<ClearDatabaseResult>
{
}

public class ClearDatabaseResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class ClearDatabaseHandler : IRequestHandler<ClearDatabaseRequest, ClearDatabaseResult>
{
    private readonly IDbMaintenance _dbMaintenance;
    private readonly ILogger<ClearDatabaseHandler> _logger;

    public ClearDatabaseHandler(IDbMaintenance dbMaintenance, ILogger<ClearDatabaseHandler> logger)
    {
        _dbMaintenance = dbMaintenance;
        _logger = logger;
    }

    public async Task<ClearDatabaseResult> Handle(ClearDatabaseRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbMaintenance.ClearDatabaseAsync().ConfigureAwait(false);

            return new ClearDatabaseResult
            {
                Success = true,
                Message = "Database cleared and schema reapplied."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear database.");
            return new ClearDatabaseResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}