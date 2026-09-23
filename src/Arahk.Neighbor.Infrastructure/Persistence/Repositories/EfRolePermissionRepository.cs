using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arahk.Neighbor.Infrastructure.Persistence.Repositories;

public class EfRolePermissionRepository : IRolePermissionRepository
{
    private readonly NeighborDbContext _db;

    public EfRolePermissionRepository(NeighborDbContext db) => _db = db;

    public async Task<IReadOnlyList<RolePermissionGrant>> ListByCommunityAsync(
        Guid communityId,
        CancellationToken ct = default)
    {
        return await _db.RolePermissionGrants
            .Where(g => g.CommunityId == communityId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RolePermissionGrant>> ListByRoleAsync(
        Guid communityId,
        string roleKey,
        CancellationToken ct = default)
    {
        return await _db.RolePermissionGrants
            .Where(g => g.CommunityId == communityId && g.RoleKey == roleKey)
            .ToListAsync(ct);
    }

    public Task<RolePermissionGrant?> GetAsync(
        Guid communityId,
        string roleKey,
        string permissionKey,
        CancellationToken ct = default) =>
        _db.RolePermissionGrants.FirstOrDefaultAsync(
            g => g.CommunityId == communityId &&
                 g.RoleKey == roleKey &&
                 g.PermissionKey == permissionKey,
            ct);

    public async Task UpsertAsync(RolePermissionGrant grant, CancellationToken ct = default)
    {
        await UpsertCoreAsync(grant, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpsertManyAsync(
        IEnumerable<RolePermissionGrant> grants,
        CancellationToken ct = default)
    {
        foreach (var grant in grants)
            await UpsertCoreAsync(grant, ct);
        await _db.SaveChangesAsync(ct);
    }

    private async Task UpsertCoreAsync(RolePermissionGrant grant, CancellationToken ct)
    {
        var tracked = await _db.RolePermissionGrants.FindAsync(
            [grant.CommunityId, grant.RoleKey, grant.PermissionKey],
            ct);

        if (tracked is null)
        {
            _db.RolePermissionGrants.Add(grant);
        }
        else if (!ReferenceEquals(tracked, grant))
        {
            tracked.SetEnabled(grant.IsEnabled, grant.UpdatedAt);
        }
    }
}
