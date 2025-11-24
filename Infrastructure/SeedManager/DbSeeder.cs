using Application.Services;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SeedManager;

internal class DbSeeder : IDbSeeder
{
    private readonly DataContext _context;
    private readonly HotelSeeder _hotelSeeder;

    public DbSeeder(DataContext context, HotelSeeder hotelSeeder)
    {
        _context = context;
        _hotelSeeder = hotelSeeder;
    }

    public async Task SeedDemoDataAsync()
    {
        // Only seed if Hotel table is empty (same logic as before)
        if (!await _context.Hotel.AnyAsync().ConfigureAwait(false))
        {
            await _hotelSeeder.GenerateDataAsync().ConfigureAwait(false);
        }
    }
}