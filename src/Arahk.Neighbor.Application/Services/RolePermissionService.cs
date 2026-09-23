using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Domain.Catalog;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Application.Services;

public class RolePermissionService
{
    private readonly IPermissionMasterRepository _masters;
    private readonly IRolePermissionRepository _grants;
    private readonly IClock _clock;

    public RolePermissionService(
        IPermissionMasterRepository masters,
        IRolePermissionRepository grants,
        IClock clock)
    {
        _masters = masters;
        _grants = grants;
        _clock = clock;
    }

    public Task<IReadOnlyList<RoleDto>> ListRolesAsync(CancellationToken ct = default)
    {
        IReadOnlyList<RoleDto> list = RoleCatalog.All
            .OrderBy(r => r.SortOrder)
            .Select(r => new RoleDto(r.Key, r.NameTh, r.NameEn, r.SortOrder))
            .ToList();
        return Task.FromResult(list);
    }

    public async Task<PermissionCatalogSummaryDto> GetCatalogAsync(
        Guid communityId,
        CancellationToken ct = default)
    {
        var masterMap = await LoadMasterMapAsync(communityId, ct);
        var grants = await _grants.ListByCommunityAsync(communityId, ct);

        var items = new List<PermissionCatalogItemDto>();
        foreach (var def in PermissionCatalog.All.OrderBy(p => p.SortOrder))
        {
            var enabled = masterMap.TryGetValue(def.Key, out var m) ? m : true;
            var usingRoles = grants
                .Where(g => g.PermissionKey == def.Key && g.IsEnabled)
                .Select(g => g.RoleKey)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(k => RoleCatalog.Find(k)?.SortOrder ?? 99)
                .ToList();

            items.Add(new PermissionCatalogItemDto(
                def.Key,
                def.NameTh,
                def.Group,
                def.IsProposed,
                enabled,
                usingRoles,
                def.SortOrder));
        }

        var enabledCount = items.Count(i => i.IsEnabledInSystem);
        return new PermissionCatalogSummaryDto(
            items,
            items.Count,
            enabledCount,
            items.Count - enabledCount);
    }

    public async Task<Result> SetMasterEnabledAsync(
        Guid communityId,
        string permissionKey,
        bool enabled,
        CancellationToken ct = default)
    {
        if (PermissionCatalog.Find(permissionKey) is null)
            return Result.Fail("perms.error.unknown_key");

        var now = _clock.UtcNow;
        var existing = await _masters.GetAsync(communityId, permissionKey, ct);
        if (existing is null)
        {
            existing = PermissionMasterState.Create(communityId, permissionKey, enabled, now);
        }
        else
        {
            existing.SetEnabled(enabled, now);
        }

        await _masters.UpsertAsync(existing, ct);
        return Result.Ok();
    }

    public async Task<Result<RolePermissionDetailDto>> GetRolePermissionsAsync(
        Guid communityId,
        string roleKey,
        CancellationToken ct = default)
    {
        var role = RoleCatalog.Find(roleKey);
        if (role is null)
            return Result<RolePermissionDetailDto>.Fail("roles.error.unknown_role");

        var masterMap = await LoadMasterMapAsync(communityId, ct);
        var grants = await _grants.ListByRoleAsync(communityId, roleKey, ct);
        var grantMap = grants.ToDictionary(g => g.PermissionKey, g => g.IsEnabled, StringComparer.Ordinal);

        var rows = PermissionCatalog.All
            .OrderBy(p => p.SortOrder)
            .Select(p =>
            {
                var masterOn = masterMap.TryGetValue(p.Key, out var m) ? m : true;
                var granted = grantMap.TryGetValue(p.Key, out var g) && g;
                return new RolePermissionRowDto(
                    p.Key,
                    p.NameTh,
                    p.Group,
                    p.IsProposed,
                    masterOn,
                    granted,
                    IsLocked: !masterOn,
                    p.SortOrder);
            })
            .ToList();

        return Result<RolePermissionDetailDto>.Ok(new RolePermissionDetailDto(
            new RoleDto(role.Key, role.NameTh, role.NameEn, role.SortOrder),
            rows));
    }

    /// <summary>
    /// Persists per-role grants. Locked (master OFF) keys are ignored so
    /// previous values remain for restore when master is re-enabled.
    /// </summary>
    public async Task<Result> SaveRolePermissionsAsync(
        Guid communityId,
        SaveRolePermissionsRequest request,
        CancellationToken ct = default)
    {
        if (RoleCatalog.Find(request.RoleKey) is null)
            return Result.Fail("roles.error.unknown_role");

        var masterMap = await LoadMasterMapAsync(communityId, ct);
        var now = _clock.UtcNow;
        var toUpsert = new List<RolePermissionGrant>();

        foreach (var (key, enabled) in request.Grants)
        {
            if (PermissionCatalog.Find(key) is null)
                continue;

            var masterOn = masterMap.TryGetValue(key, out var m) ? m : true;
            if (!masterOn)
                continue; // keep previous stored value

            var existing = await _grants.GetAsync(communityId, request.RoleKey, key, ct);
            if (existing is null)
            {
                toUpsert.Add(RolePermissionGrant.Create(
                    communityId, request.RoleKey, key, enabled, now));
            }
            else if (existing.IsEnabled != enabled)
            {
                existing.SetEnabled(enabled, now);
                toUpsert.Add(existing);
            }
        }

        if (toUpsert.Count > 0)
            await _grants.UpsertManyAsync(toUpsert, ct);

        return Result.Ok();
    }

    /// <summary>Effective access = master ON and role grant ON.</summary>
    public async Task<bool> IsEffectivelyGrantedAsync(
        Guid communityId,
        string roleKey,
        string permissionKey,
        CancellationToken ct = default)
    {
        var master = await _masters.GetAsync(communityId, permissionKey, ct);
        var masterOn = master?.IsEnabledInSystem ?? true;
        if (!masterOn)
            return false;

        var grant = await _grants.GetAsync(communityId, roleKey, permissionKey, ct);
        return grant?.IsEnabled ?? false;
    }

    private async Task<Dictionary<string, bool>> LoadMasterMapAsync(
        Guid communityId,
        CancellationToken ct)
    {
        var list = await _masters.ListByCommunityAsync(communityId, ct);
        return list.ToDictionary(
            m => m.PermissionKey,
            m => m.IsEnabledInSystem,
            StringComparer.Ordinal);
    }
}
