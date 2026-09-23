using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Application.Interfaces;

public interface IPermissionMasterRepository
{
    Task<IReadOnlyList<PermissionMasterState>> ListByCommunityAsync(
        Guid communityId,
        CancellationToken ct = default);

    Task<PermissionMasterState?> GetAsync(
        Guid communityId,
        string permissionKey,
        CancellationToken ct = default);

    Task UpsertAsync(PermissionMasterState state, CancellationToken ct = default);
}
