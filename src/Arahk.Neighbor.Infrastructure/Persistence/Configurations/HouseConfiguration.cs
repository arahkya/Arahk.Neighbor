using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arahk.Neighbor.Infrastructure.Persistence.Configurations;

public class HouseConfiguration : IEntityTypeConfiguration<House>
{
    public void Configure(EntityTypeBuilder<House> builder)
    {
        builder.ToTable("Houses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CommunityId).IsRequired();
        builder.Property(x => x.HouseNo)
            .IsRequired()
            .HasMaxLength(HouseConstants.HouseNoMaxLength);
        builder.Property(x => x.Soi).HasMaxLength(HouseConstants.SoiMaxLength);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.CommunityId, x.HouseNo }).IsUnique();
    }
}
