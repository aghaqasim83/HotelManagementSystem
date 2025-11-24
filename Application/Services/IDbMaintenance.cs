namespace Application.Services;

public interface IDbMaintenance
{
    /// <summary>
    /// Clear data from the database.
    /// Implementation lives in Infrastructure and should handle migrations vs EnsureCreated.
    /// </summary>
    Task ClearDatabaseAsync();
}