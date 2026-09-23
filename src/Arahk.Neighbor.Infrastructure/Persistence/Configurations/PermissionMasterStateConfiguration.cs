using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arahk.Neighbor.Infrastructure.Persistence.Configurations;

public class PermissionMasterStateConfiguration : IEntityTypeConfiguration<PermissionMasterState>
{
    public void Configure(EntityTypeBuilder<PermissionMasterState> builder)
    {
        builder.ToTable("PermissionMasters");
        builder.HasKey(x => new { x.CommunityId, x.PermissionKey });

        builder.Property(x => x.PermissionKey)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.IsEnabledInSystem).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}
