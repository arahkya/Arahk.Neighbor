using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arahk.Neighbor.Infrastructure.Persistence.Repositories;

public class EfUserRepository : IUserRepository
{
    private readonly NeighborDbContext _db;

    public EfUserRepository(NeighborDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _db.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
    }

    public Task<User?> GetByPhoneLocalAsync(string phoneLocal, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.PhoneLocal == phoneLocal, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        var entry = _db.Entry(user);
        if (entry.State == EntityState.Detached)
            _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }
}
