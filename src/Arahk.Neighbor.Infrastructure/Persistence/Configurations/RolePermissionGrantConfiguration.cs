using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arahk.Neighbor.Infrastructure.Persistence.Configurations;

public class RolePermissionGrantConfiguration : IEntityTypeConfiguration<RolePermissionGrant>
{
    public void Configure(EntityTypeBuilder<RolePermissionGrant> builder)
    {
        builder.ToTable("RolePermissionGrants");
        builder.HasKey(x => new { x.CommunityId, x.RoleKey, x.PermissionKey });

        builder.Property(x => x.RoleKey)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.PermissionKey)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.IsEnabled).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}
