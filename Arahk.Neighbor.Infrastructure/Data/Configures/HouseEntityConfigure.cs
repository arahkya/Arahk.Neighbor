using Arahk.Neighbor.Domain.Administrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arahk.Neighbor.Infrastructure.Data.Configures;

public class HouseEntityConfigure : IEntityTypeConfiguration<HouseEntity>
{
    public void Configure(EntityTypeBuilder<HouseEntity> builder)
    {
        builder.ToTable("Houses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AddressName).IsRequired().HasMaxLength(50);
    }
}
