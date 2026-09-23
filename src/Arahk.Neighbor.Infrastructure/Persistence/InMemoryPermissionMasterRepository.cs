using System.Collections.Concurrent;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Infrastructure.Persistence;

public class InMemoryPermissionMasterRepository : IPermissionMasterRepository
{
    private readonly ConcurrentDictionary<(Guid CommunityId, string Key), PermissionMasterState> _store = new();

    public Task<IReadOnlyList<PermissionMasterState>> ListByCommunityAsync(
        Guid communityId,
        CancellationToken ct = default)
    {
        IReadOnlyList<PermissionMasterState> list = _store.Values
            .Where(s => s.CommunityId == communityId)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<PermissionMasterState?> GetAsync(
        Guid communityId,
        string permissionKey,
        CancellationToken ct = default)
    {
        _store.TryGetValue((communityId, permissionKey), out var state);
        return Task.FromResult(state);
    }

    public Task UpsertAsync(PermissionMasterState state, CancellationToken ct = default)
    {
        _store[(state.CommunityId, state.PermissionKey)] = state;
        return Task.CompletedTask;
    }
}
