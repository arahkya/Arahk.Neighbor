using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arahk.Neighbor.Infrastructure.Persistence.Configurations;

public class EmailOtpConfiguration : IEntityTypeConfiguration<EmailOtp>
{
    public void Configure(EntityTypeBuilder<EmailOtp> builder)
    {
        builder.ToTable("EmailOtps");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(AuthConstants.EmailMaxLength);
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(AuthConstants.OtpLength);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.ResendAvailableAt).IsRequired();
        builder.Property(x => x.FailedAttempts).IsRequired();
        builder.Property(x => x.IsConsumed).IsRequired();
        builder.Property(x => x.IsInvalidated).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.IsConsumed, x.IsInvalidated });
    }
}
