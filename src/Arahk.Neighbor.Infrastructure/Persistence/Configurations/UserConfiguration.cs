using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arahk.Neighbor.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(AuthConstants.DisplayNameMaxLength);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(AuthConstants.EmailMaxLength);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.PhoneE164).HasMaxLength(16);
        builder.Property(x => x.PhoneLocal).HasMaxLength(16);
        builder.HasIndex(x => x.PhoneLocal);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.EmailVerified).IsRequired();
        builder.Property(x => x.TermsAccepted).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
