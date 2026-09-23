using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arahk.Neighbor.Infrastructure.Persistence.Repositories;

public class EfPermissionMasterRepository : IPermissionMasterRepository
{
    private readonly NeighborDbContext _db;

    public EfPermissionMasterRepository(NeighborDbContext db) => _db = db;

    public async Task<IReadOnlyList<PermissionMasterState>> ListByCommunityAsync(
        Guid communityId,
        CancellationToken ct = default)
    {
        return await _db.PermissionMasters
            .Where(s => s.CommunityId == communityId)
            .ToListAsync(ct);
    }

    public Task<PermissionMasterState?> GetAsync(
        Guid communityId,
        string permissionKey,
        CancellationToken ct = default) =>
        _db.PermissionMasters.FirstOrDefaultAsync(
            s => s.CommunityId == communityId && s.PermissionKey == permissionKey,
            ct);

    public async Task UpsertAsync(PermissionMasterState state, CancellationToken ct = default)
    {
        var tracked = await _db.PermissionMasters.FindAsync(
            [state.CommunityId, state.PermissionKey],
            ct);

        if (tracked is null)
        {
            _db.PermissionMasters.Add(state);
        }
        else if (!ReferenceEquals(tracked, state))
        {
            tracked.SetEnabled(state.IsEnabledInSystem, state.UpdatedAt);
        }

        await _db.SaveChangesAsync(ct);
    }
}
