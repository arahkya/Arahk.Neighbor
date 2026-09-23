using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Application.Interfaces;

public interface IRolePermissionRepository
{
    Task<IReadOnlyList<RolePermissionGrant>> ListByCommunityAsync(
        Guid communityId,
        CancellationToken ct = default);

    Task<IReadOnlyList<RolePermissionGrant>> ListByRoleAsync(
        Guid communityId,
        string roleKey,
        CancellationToken ct = default);

    Task<RolePermissionGrant?> GetAsync(
        Guid communityId,
        string roleKey,
        string permissionKey,
        CancellationToken ct = default);

    Task UpsertAsync(RolePermissionGrant grant, CancellationToken ct = default);

    Task UpsertManyAsync(IEnumerable<RolePermissionGrant> grants, CancellationToken ct = default);
}
