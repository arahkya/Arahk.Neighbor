using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arahk.Neighbor.Infrastructure.Persistence.Repositories;

public class EfOtpRepository : IOtpRepository
{
    private readonly NeighborDbContext _db;

    public EfOtpRepository(NeighborDbContext db) => _db = db;

    public async Task<EmailOtp?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        // SQLite cannot ORDER BY DateTimeOffset in SQL — order on client (few rows per user).
        var candidates = await _db.EmailOtps
            .Where(o => o.UserId == userId && !o.IsConsumed && !o.IsInvalidated)
            .ToListAsync(ct);

        return candidates
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefault();
    }

    public async Task AddAsync(EmailOtp otp, CancellationToken ct = default)
    {
        _db.EmailOtps.Add(otp);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(EmailOtp otp, CancellationToken ct = default)
    {
        var entry = _db.Entry(otp);
        if (entry.State == EntityState.Detached)
            _db.EmailOtps.Update(otp);
        await _db.SaveChangesAsync(ct);
    }

    public async Task InvalidateActiveForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var active = await _db.EmailOtps
            .Where(o => o.UserId == userId && !o.IsConsumed && !o.IsInvalidated)
            .ToListAsync(ct);

        foreach (var otp in active)
            otp.Invalidate();

        if (active.Count > 0)
            await _db.SaveChangesAsync(ct);
    }
}
