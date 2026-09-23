using System.Collections.Concurrent;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Infrastructure.Persistence;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _byId = new();

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _byId.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = _byId.Values.FirstOrDefault(u => u.Email == normalized);
        return Task.FromResult(user);
    }

    public Task<User?> GetByPhoneLocalAsync(string phoneLocal, CancellationToken ct = default)
    {
        var user = _byId.Values.FirstOrDefault(u => u.PhoneLocal == phoneLocal);
        return Task.FromResult(user);
    }

    public Task AddAsync(User user, CancellationToken ct = default)
    {
        if (!_byId.TryAdd(user.Id, user))
            throw new InvalidOperationException("User already exists.");
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _byId[user.Id] = user;
        return Task.CompletedTask;
    }
}
