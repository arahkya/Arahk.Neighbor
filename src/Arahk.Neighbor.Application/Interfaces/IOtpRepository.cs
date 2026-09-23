using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Application.Interfaces;

public interface IOtpRepository
{
    Task<EmailOtp?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(EmailOtp otp, CancellationToken ct = default);
    Task UpdateAsync(EmailOtp otp, CancellationToken ct = default);
    Task InvalidateActiveForUserAsync(Guid userId, CancellationToken ct = default);
}
