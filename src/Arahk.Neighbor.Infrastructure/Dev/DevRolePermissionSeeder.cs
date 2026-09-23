using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Catalog;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arahk.Neighbor.Infrastructure.Dev;

/// <summary>
/// Idempotent Development seed for 5 roles + permission catalog + defaults.
/// </summary>
public static class DevRolePermissionSeeder
{
    public static async Task<bool> SeedIfDevelopmentAsync(
        IServiceProvider services,
        IHostEnvironment environment,
        Guid? communityId = null,
        CancellationToken ct = default)
    {
        if (!environment.IsDevelopment())
            return false;

        await using var scope = services.CreateAsyncScope();
        var masters = scope.ServiceProvider.GetRequiredService<IPermissionMasterRepository>();
        var grants = scope.ServiceProvider.GetRequiredService<IRolePermissionRepository>();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();
        return await EnsureSeedAsync(
            masters,
            grants,
            clock,
            communityId ?? HouseConstants.DefaultCommunityId,
            ct);
    }

    /// <summary>
    /// Seeds masters (all ON) and role grants from PermissionDefaults when missing.
    /// Idempotent: never overwrites existing master or grant rows.
    /// Returns true if any row was created.
    /// </summary>
    public static async Task<bool> EnsureSeedAsync(
        IPermissionMasterRepository masters,
        IRolePermissionRepository grants,
        IClock clock,
        Guid communityId,
        CancellationToken ct = default)
    {
        var now = clock.UtcNow;
        var created = false;

        foreach (var perm in PermissionCatalog.All)
        {
            var existing = await masters.GetAsync(communityId, perm.Key, ct);
            if (existing is null)
            {
                await masters.UpsertAsync(
                    PermissionMasterState.Create(communityId, perm.Key, enabled: true, now),
                    ct);
                created = true;
            }
        }

        foreach (var role in RoleCatalog.All)
        {
            foreach (var perm in PermissionCatalog.All)
            {
                var existing = await grants.GetAsync(communityId, role.Key, perm.Key, ct);
                if (existing is null)
                {
                    var on = PermissionDefaults.IsDefaultOn(role.Key, perm.Key);
                    await grants.UpsertAsync(
                        RolePermissionGrant.Create(communityId, role.Key, perm.Key, on, now),
                        ct);
                    created = true;
                }
            }
        }

        return created;
    }
}
