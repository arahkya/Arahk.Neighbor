using System.Collections.Concurrent;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Infrastructure.Persistence;

public class InMemoryRolePermissionRepository : IRolePermissionRepository
{
    private readonly ConcurrentDictionary<(Guid CommunityId, string Role, string Perm), RolePermissionGrant> _store = new();

    public Task<IReadOnlyList<RolePermissionGrant>> ListByCommunityAsync(
        Guid communityId,
        CancellationToken ct = default)
    {
        IReadOnlyList<RolePermissionGrant> list = _store.Values
            .Where(g => g.CommunityId == communityId)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<RolePermissionGrant>> ListByRoleAsync(
        Guid communityId,
        string roleKey,
        CancellationToken ct = default)
    {
        IReadOnlyList<RolePermissionGrant> list = _store.Values
            .Where(g => g.CommunityId == communityId &&
                        string.Equals(g.RoleKey, roleKey, StringComparison.Ordinal))
            .ToList();
        return Task.FromResult(list);
    }

    public Task<RolePermissionGrant?> GetAsync(
        Guid communityId,
        string roleKey,
        string permissionKey,
        CancellationToken ct = default)
    {
        _store.TryGetValue((communityId, roleKey, permissionKey), out var grant);
        return Task.FromResult(grant);
    }

    public Task UpsertAsync(RolePermissionGrant grant, CancellationToken ct = default)
    {
        _store[(grant.CommunityId, grant.RoleKey, grant.PermissionKey)] = grant;
        return Task.CompletedTask;
    }

    public Task UpsertManyAsync(IEnumerable<RolePermissionGrant> grants, CancellationToken ct = default)
    {
        foreach (var grant in grants)
            _store[(grant.CommunityId, grant.RoleKey, grant.PermissionKey)] = grant;
        return Task.CompletedTask;
    }
}
