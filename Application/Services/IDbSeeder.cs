namespace Application.Services;

public interface IDbSeeder
{
    /// <summary>
    /// Seed demo data (if required). Implementations are responsible for checking whether seeding is necessary.
    /// </summary>
    Task SeedDemoDataAsync();
}