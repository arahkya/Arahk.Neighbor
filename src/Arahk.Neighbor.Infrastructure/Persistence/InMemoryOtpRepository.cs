using System.Collections.Concurrent;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Infrastructure.Persistence;

public class InMemoryOtpRepository : IOtpRepository
{
    private readonly ConcurrentDictionary<Guid, EmailOtp> _byId = new();

    public Task<EmailOtp?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var otp = _byId.Values
            .Where(o => o.UserId == userId && !o.IsConsumed && !o.IsInvalidated)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefault();
        return Task.FromResult(otp);
    }

    public Task AddAsync(EmailOtp otp, CancellationToken ct = default)
    {
        _byId[otp.Id] = otp;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(EmailOtp otp, CancellationToken ct = default)
    {
        _byId[otp.Id] = otp;
        return Task.CompletedTask;
    }

    public Task InvalidateActiveForUserAsync(Guid userId, CancellationToken ct = default)
    {
        foreach (var otp in _byId.Values.Where(o => o.UserId == userId && !o.IsConsumed && !o.IsInvalidated))
            otp.Invalidate();
        return Task.CompletedTask;
    }
}
