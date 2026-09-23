using Arahk.Neighbor.Domain.Constants;

namespace Arahk.Neighbor.Domain.Catalog;

/// <summary>Starter ON/OFF matrix from UX 01b-permission-defaults.md.</summary>
public static class PermissionDefaults
{
    /// <summary>
    /// Returns whether <paramref name="permissionKey"/> defaults ON for <paramref name="roleKey"/>.
    /// Unknown role/permission → false.
    /// </summary>
    public static bool IsDefaultOn(string roleKey, string permissionKey)
    {
        if (!_matrix.TryGetValue(roleKey, out var perms))
            return false;
        return perms.Contains(permissionKey);
    }

    public static IReadOnlyDictionary<string, IReadOnlySet<string>> Matrix => _matrix;

    private static readonly Dictionary<string, IReadOnlySet<string>> _matrix = new(StringComparer.Ordinal)
    {
        [RoleKeys.Resident] = new HashSet<string>(StringComparer.Ordinal)
        {
            PermissionKeys.CanLogin,
            PermissionKeys.MenuHome,
            PermissionKeys.MenuPayment,
            PermissionKeys.MenuVisitor,
            PermissionKeys.Tickets,
        },
        [RoleKeys.Juristic] = new HashSet<string>(StringComparer.Ordinal)
        {
            PermissionKeys.CanLogin,
            PermissionKeys.MenuHome,
            PermissionKeys.MenuPayment,
            PermissionKeys.MenuPackages,
            PermissionKeys.MenuVisitor,
            PermissionKeys.MasterHouses,
            PermissionKeys.MasterUsers,
            PermissionKeys.MasterRoles,
            PermissionKeys.Tickets,
        },
        [RoleKeys.Committee] = new HashSet<string>(StringComparer.Ordinal)
        {
            PermissionKeys.CanLogin,
            PermissionKeys.MenuHome,
            PermissionKeys.MenuPayment,
            PermissionKeys.MenuPackages,
            PermissionKeys.MenuVisitor,
            PermissionKeys.MasterHouses,
            PermissionKeys.MasterUsers,
            PermissionKeys.MasterRoles,
            PermissionKeys.Tickets,
        },
        [RoleKeys.Auditor] = new HashSet<string>(StringComparer.Ordinal)
        {
            PermissionKeys.CanLogin,
            PermissionKeys.MenuHome,
            PermissionKeys.MenuPayment,
            PermissionKeys.MasterHouses,
        },
        [RoleKeys.Security] = new HashSet<string>(StringComparer.Ordinal)
        {
            PermissionKeys.CanLogin,
            PermissionKeys.MenuHome,
            PermissionKeys.MenuVisitor,
            PermissionKeys.MasterHouses,
        },
    };
}
