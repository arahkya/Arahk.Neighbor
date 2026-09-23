using Arahk.Neighbor.Domain.Catalog;
using Arahk.Neighbor.Domain.Constants;
using FluentAssertions;

namespace Arahk.Neighbor.Domain.Tests;

public class PermissionCatalogTests
{
    [Fact]
    public void Catalog_has_starter_keys_from_ux()
    {
        PermissionCatalog.All.Select(p => p.Key).Should().BeEquivalentTo(
        [
            PermissionKeys.CanLogin,
            PermissionKeys.MenuHome,
            PermissionKeys.MenuPayment,
            PermissionKeys.MenuPackages,
            PermissionKeys.MenuVisitor,
            PermissionKeys.MasterHouses,
            PermissionKeys.MasterUsers,
            PermissionKeys.MasterRoles,
            PermissionKeys.Tickets,
        ]);
        PermissionCatalog.All.Should().ContainSingle(p => p.Key == PermissionKeys.Tickets && p.IsProposed);
    }

    [Fact]
    public void Role_catalog_is_exactly_five()
    {
        RoleCatalog.All.Should().HaveCount(5);
        RoleCatalog.All.Should().NotContain(r => r.Key.Contains("worker", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Defaults_match_01b_matrix()
    {
        PermissionDefaults.IsDefaultOn(RoleKeys.Resident, PermissionKeys.MenuPackages).Should().BeFalse();
        PermissionDefaults.IsDefaultOn(RoleKeys.Juristic, PermissionKeys.MasterRoles).Should().BeTrue();
        PermissionDefaults.IsDefaultOn(RoleKeys.Security, PermissionKeys.MenuPayment).Should().BeFalse();
        PermissionDefaults.IsDefaultOn(RoleKeys.Auditor, PermissionKeys.MenuVisitor).Should().BeFalse();
        PermissionDefaults.IsDefaultOn(RoleKeys.Committee, PermissionKeys.Tickets).Should().BeTrue();
    }
}
