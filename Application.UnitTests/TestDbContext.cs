using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests;

// Small test DbContext used to produce IQueryable<T> backed by EF Core's provider
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options) : base(options) { }

    // Include entity types in the model so EF Core can create DbSet for them
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Hotel> Hotels { get; set; } = null!;
}
