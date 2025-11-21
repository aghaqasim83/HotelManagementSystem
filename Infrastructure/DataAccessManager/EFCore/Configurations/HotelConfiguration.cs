using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants;

namespace Infrastructure.DataAccessManager.EFCore.Configurations;

public class HotelConfiguration : BaseEntityConfiguration<Hotel>
{
    public override void Configure(EntityTypeBuilder<Hotel> builder)
    {
        // apply base mappings for BaseEntity (Id, audit, etc.)
        base.Configure(builder);

        // Id: BaseEntity uses a string (sequential GUID) - keep as required nvarchar(36)
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
               .HasMaxLength(36)
               .IsRequired();

        // Name
        builder.Property(x => x.Name)
               .HasMaxLength(NameConsts.MaxLength)
               .IsRequired(false);

        builder.HasIndex(e => e.Name);

        // Rooms relationship: one Hotel has many Rooms
        // Ensure FK on Room (HotelId) maps to Hotel.Id (both are string)
        builder.HasMany(h => h.Rooms)
               .WithOne(r => r.Hotel)
               .HasForeignKey(r => r.HotelId)
               .OnDelete(DeleteBehavior.Cascade);

        // Audit columns mapping (ensure appropriate SQL types)
        builder.Property(b => b.CreatedAtUtc).HasColumnType("datetime2").IsRequired(false);
        builder.Property(b => b.UpdatedAtUtc).HasColumnType("datetime2").IsRequired(false);

        // Soft-delete flag
        builder.Property(b => b.IsDeleted).IsRequired();
    }
}