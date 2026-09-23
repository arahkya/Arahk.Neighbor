using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Domain.Catalog;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Arahk.Neighbor.Application.Tests;

public class RolePermissionServiceTests
{
    private readonly Guid _community = HouseConstants.DefaultCommunityId;
    private readonly Mock<IPermissionMasterRepository> _masters = new();
    private readonly Mock<IRolePermissionRepository> _grants = new();
    private readonly Mock<IClock> _clock = new();
    private readonly RolePermissionService _sut;
    private readonly DateTimeOffset _now = DateTimeOffset.Parse("2026-09-23T10:00:00Z");

    private readonly Dictionary<string, PermissionMasterState> _masterStore = new(StringComparer.Ordinal);
    private readonly Dictionary<(string Role, string Perm), RolePermissionGrant> _grantStore = new();

    public RolePermissionServiceTests()
    {
        _clock.Setup(c => c.UtcNow).Returns(_now);
        WireMasters();
        WireGrants();
        _sut = new RolePermissionService(_masters.Object, _grants.Object, _clock.Object);
    }

    private void WireMasters()
    {
        _masters.Setup(m => m.ListByCommunityAsync(_community, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => _masterStore.Values.ToList());
        _masters.Setup(m => m.GetAsync(_community, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, string key, CancellationToken _) =>
                _masterStore.TryGetValue(key, out var s) ? s : null);
        _masters.Setup(m => m.UpsertAsync(It.IsAny<PermissionMasterState>(), It.IsAny<CancellationToken>()))
            .Callback<PermissionMasterState, CancellationToken>((s, _) => _masterStore[s.PermissionKey] = s)
            .Returns(Task.CompletedTask);
    }

    private void WireGrants()
    {
        _grants.Setup(g => g.ListByCommunityAsync(_community, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => _grantStore.Values.ToList());
        _grants.Setup(g => g.ListByRoleAsync(_community, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, string role, CancellationToken _) =>
                _grantStore.Values.Where(x => x.RoleKey == role).ToList());
        _grants.Setup(g => g.GetAsync(_community, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, string role, string perm, CancellationToken _) =>
                _grantStore.TryGetValue((role, perm), out var g) ? g : null);
        _grants.Setup(g => g.UpsertAsync(It.IsAny<RolePermissionGrant>(), It.IsAny<CancellationToken>()))
            .Callback<RolePermissionGrant, CancellationToken>((g, _) =>
                _grantStore[(g.RoleKey, g.PermissionKey)] = g)
            .Returns(Task.CompletedTask);
        _grants.Setup(g => g.UpsertManyAsync(It.IsAny<IEnumerable<RolePermissionGrant>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<RolePermissionGrant>, CancellationToken>((list, _) =>
            {
                foreach (var g in list)
                    _grantStore[(g.RoleKey, g.PermissionKey)] = g;
            })
            .Returns(Task.CompletedTask);
    }

    private void SeedGrant(string role, string perm, bool on) =>
        _grantStore[(role, perm)] = RolePermissionGrant.Create(_community, role, perm, on, _now);

    private void SeedMaster(string perm, bool on) =>
        _masterStore[perm] = PermissionMasterState.Create(_community, perm, on, _now);

    [Fact]
    public async Task ListRoles_returns_exactly_five_without_internal_worker()
    {
        var roles = await _sut.ListRolesAsync();
        roles.Should().HaveCount(5);
        roles.Select(r => r.Key).Should().BeEquivalentTo(
        [
            RoleKeys.Resident, RoleKeys.Juristic, RoleKeys.Committee,
            RoleKeys.Auditor, RoleKeys.Security
        ]);
        roles.Should().NotContain(r => r.Key.Contains("worker", StringComparison.OrdinalIgnoreCase));
        roles[0].NameTh.Should().Be("ลูกบ้าน");
        roles[1].NameTh.Should().Be("นิติบุคคล");
    }

    [Fact]
    public async Task GetCatalog_lists_static_permissions_with_master_flags()
    {
        SeedMaster(PermissionKeys.MasterRoles, false);
        SeedGrant(RoleKeys.Juristic, PermissionKeys.MasterRoles, true);

        var catalog = await _sut.GetCatalogAsync(_community);
        catalog.TotalCount.Should().Be(PermissionCatalog.All.Count);
        catalog.DisabledCount.Should().Be(1);
        var masterRoles = catalog.Items.Single(i => i.Key == PermissionKeys.MasterRoles);
        masterRoles.IsEnabledInSystem.Should().BeFalse();
        masterRoles.RoleKeysUsing.Should().Contain(RoleKeys.Juristic);
        catalog.Items.Should().Contain(i => i.Key == PermissionKeys.Tickets && i.IsProposed);
    }

    [Fact]
    public async Task Master_off_locks_effective_access_but_preserves_grant()
    {
        SeedMaster(PermissionKeys.MenuPackages, true);
        SeedGrant(RoleKeys.Juristic, PermissionKeys.MenuPackages, true);

        (await _sut.IsEffectivelyGrantedAsync(_community, RoleKeys.Juristic, PermissionKeys.MenuPackages))
            .Should().BeTrue();

        var off = await _sut.SetMasterEnabledAsync(_community, PermissionKeys.MenuPackages, false);
        off.Succeeded.Should().BeTrue();

        (await _sut.IsEffectivelyGrantedAsync(_community, RoleKeys.Juristic, PermissionKeys.MenuPackages))
            .Should().BeFalse();

        var detail = await _sut.GetRolePermissionsAsync(_community, RoleKeys.Juristic);
        detail.Succeeded.Should().BeTrue();
        var row = detail.Data!.Permissions.Single(p => p.Key == PermissionKeys.MenuPackages);
        row.IsLocked.Should().BeTrue();
        row.IsMasterEnabled.Should().BeFalse();
        row.IsGranted.Should().BeTrue(); // previous value preserved
    }

    [Fact]
    public async Task Master_re_enabled_restores_previous_role_values()
    {
        SeedMaster(PermissionKeys.MenuPackages, true);
        SeedGrant(RoleKeys.Juristic, PermissionKeys.MenuPackages, true);
        SeedGrant(RoleKeys.Resident, PermissionKeys.MenuPackages, false);

        await _sut.SetMasterEnabledAsync(_community, PermissionKeys.MenuPackages, false);
        await _sut.SetMasterEnabledAsync(_community, PermissionKeys.MenuPackages, true);

        (await _sut.IsEffectivelyGrantedAsync(_community, RoleKeys.Juristic, PermissionKeys.MenuPackages))
            .Should().BeTrue();
        (await _sut.IsEffectivelyGrantedAsync(_community, RoleKeys.Resident, PermissionKeys.MenuPackages))
            .Should().BeFalse();

        var neverSet = await _sut.IsEffectivelyGrantedAsync(
            _community, RoleKeys.Security, PermissionKeys.MenuPackages);
        neverSet.Should().BeFalse(); // never set = OFF
    }

    [Fact]
    public async Task SaveRolePermissions_persists_toggles_for_unlocked_only()
    {
        SeedMaster(PermissionKeys.MenuPayment, true);
        SeedMaster(PermissionKeys.MenuPackages, false);
        SeedGrant(RoleKeys.Security, PermissionKeys.MenuPayment, false);
        SeedGrant(RoleKeys.Security, PermissionKeys.MenuPackages, true); // preserved while locked

        var result = await _sut.SaveRolePermissionsAsync(_community, new SaveRolePermissionsRequest(
            RoleKeys.Security,
            new Dictionary<string, bool>
            {
                [PermissionKeys.MenuPayment] = true,
                [PermissionKeys.MenuPackages] = false, // attempt to change locked — ignored
            }));

        result.Succeeded.Should().BeTrue();
        _grantStore[(RoleKeys.Security, PermissionKeys.MenuPayment)].IsEnabled.Should().BeTrue();
        _grantStore[(RoleKeys.Security, PermissionKeys.MenuPackages)].IsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Save_unknown_role_fails()
    {
        var result = await _sut.SaveRolePermissionsAsync(_community, new SaveRolePermissionsRequest(
            "internal_worker",
            new Dictionary<string, bool> { [PermissionKeys.CanLogin] = true }));
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task Role_toggles_roundtrip_via_get_and_save()
    {
        SeedMaster(PermissionKeys.Tickets, true);
        SeedGrant(RoleKeys.Auditor, PermissionKeys.Tickets, false);

        var before = await _sut.GetRolePermissionsAsync(_community, RoleKeys.Auditor);
        before.Data!.Permissions.Single(p => p.Key == PermissionKeys.Tickets).IsGranted.Should().BeFalse();

        await _sut.SaveRolePermissionsAsync(_community, new SaveRolePermissionsRequest(
            RoleKeys.Auditor,
            new Dictionary<string, bool> { [PermissionKeys.Tickets] = true }));

        var after = await _sut.GetRolePermissionsAsync(_community, RoleKeys.Auditor);
        after.Data!.Permissions.Single(p => p.Key == PermissionKeys.Tickets).IsGranted.Should().BeTrue();
    }
}

public class RolePermissionDraftTests
{
    [Fact]
    public void Dirty_cancel_restores_baseline_without_persisting()
    {
        var draft = new RolePermissionDraft(new Dictionary<string, bool>
        {
            ["can_login"] = true,
            ["menu_home"] = true,
        });

        draft.IsDirty.Should().BeFalse();
        draft.TrySet("menu_home", false).Should().BeTrue();
        draft.IsDirty.Should().BeTrue();
        draft.Current["menu_home"].Should().BeFalse();

        draft.Cancel();
        draft.IsDirty.Should().BeFalse();
        draft.Current["menu_home"].Should().BeTrue();
    }

    [Fact]
    public void Locked_keys_cannot_be_toggled()
    {
        var draft = new RolePermissionDraft(
            new Dictionary<string, bool> { ["menu_packages"] = true },
            lockedPermissionKeys: ["menu_packages"]);

        draft.TrySet("menu_packages", false).Should().BeFalse();
        draft.Current["menu_packages"].Should().BeTrue();
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void MarkSaved_clears_dirty()
    {
        var draft = new RolePermissionDraft(new Dictionary<string, bool> { ["tickets"] = false });
        draft.TrySet("tickets", true);
        draft.IsDirty.Should().BeTrue();
        draft.MarkSaved();
        draft.IsDirty.Should().BeFalse();
    }
}
