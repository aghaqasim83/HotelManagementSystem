using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManager.EFCore.Configurations;

public class RoomConfiguration : BaseEntityConfiguration<Room>
{
    public override void Configure(EntityTypeBuilder<Room> builder)
    {
        // apply base mappings from BaseEntity (Id, audit, etc.)
        base.Configure(builder);

        // Id: ensure string GUID length matches BaseEntity / Hotel configuration
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
               .HasMaxLength(36)
               .IsRequired();

        // Room number (e.g. "101")
        builder.Property(x => x.Number)
               .HasMaxLength(50)
               .IsRequired();

        // Room type (e.g. "Single", "Double", "Deluxe")
        builder.Property(x => x.Type)
               .HasMaxLength(50)
               .IsRequired(false);

        // Capacity must be present
        builder.Property(x => x.Capacity)
               .IsRequired();

        // HotelId: ensure FK type/length matches Hotel.Id (string GUID)
        builder.Property(r => r.HotelId)
               .HasMaxLength(36)
               .IsRequired();

        // Index room number for lookups (consider unique per hotel if desired)
        builder.HasIndex(e => e.Number);

        // Relationship: Room -> Hotel (many rooms per hotel)
        builder.HasOne(r => r.Hotel)
               .WithMany(h => h.Rooms)
               .HasForeignKey(r => r.HotelId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Room -> Bookings
        builder.HasMany(r => r.Bookings)
               .WithOne(b => b.Room)
               .HasForeignKey(b => b.RoomId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}