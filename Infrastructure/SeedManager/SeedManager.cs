using Application.Services;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.SeedManager;

internal class DbMaintenance : IDbMaintenance
{
    private readonly DataContext _context;
    private readonly ILogger<DbMaintenance> _logger;

    public DbMaintenance(DataContext context, ILogger<DbMaintenance> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ClearDatabaseAsync()
    {
        _logger.LogInformation("Clearing data from tables (Bookings, Rooms, Hotels)...");

        // Use a transaction to ensure all deletes succeed or none are applied.
        await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
        try
        {
            // EF Core 7+ supports ExecuteDeleteAsync which issues a set-based DELETE at the database
            // without loading entities into memory. This keeps schema intact while removing data.
            // Delete child tables first to avoid FK conflicts.
            if (_context.Booking != null)
            {
                await _context.Booking.ExecuteDeleteAsync().ConfigureAwait(false);
            }

            if (_context.Room != null)
            {
                await _context.Room.ExecuteDeleteAsync().ConfigureAwait(false);
            }

            if (_context.Hotel != null)
            {
                await _context.Hotel.ExecuteDeleteAsync().ConfigureAwait(false);
            }

            await transaction.CommitAsync().ConfigureAwait(false);

            _logger.LogInformation("Data cleared successfully (tables preserved).");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear data. Rolling back transaction.");
            try
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "Rollback failed.");
            }

            throw;
        }
    }
}