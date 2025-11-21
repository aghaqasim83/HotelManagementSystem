using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManager.EFCore.Configurations;

public class BookingConfiguration : BaseEntityConfiguration<Booking>
{
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        // apply base mappings from BaseEntity (Id, audit, etc.)
        base.Configure(builder);

        // Id: ensure string GUID length matches BaseEntity / Hotel / Room configuration
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
               .HasMaxLength(36)
               .IsRequired();

        // Unique booking reference
        builder.Property(b => b.BookingReference)
               .HasMaxLength(100)
               .IsRequired();
        builder.HasIndex(b => b.BookingReference).IsUnique();

        // Guest
        builder.Property(b => b.GuestName)
               .HasMaxLength(200)
               .IsRequired();
        builder.HasIndex(b => b.GuestName);

        // Dates stored as SQL date
        builder.Property(b => b.CheckIn)
               .HasColumnType("date")
               .IsRequired();

        builder.Property(b => b.CheckOut)
               .HasColumnType("date")
               .IsRequired();

        // Counts
        builder.Property(b => b.NumberOfGuests)
               .IsRequired();

        // RoomId: ensure FK type/length matches Room.Id (string GUID)
        builder.Property(b => b.RoomId)
               .HasMaxLength(36)
               .IsRequired();

        // Relationships
        builder.HasOne(b => b.Room)
               .WithMany(r => r.Bookings)
               .HasForeignKey(b => b.RoomId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}