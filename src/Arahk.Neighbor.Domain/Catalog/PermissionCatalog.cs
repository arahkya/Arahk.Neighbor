using Arahk.Neighbor.Domain.Constants;

namespace Arahk.Neighbor.Domain.Catalog;

public sealed record PermissionDefinition(
    string Key,
    string NameTh,
    string Group,
    bool IsProposed,
    int SortOrder);

/// <summary>Static starter permission catalog — no add/delete in UI.</summary>
public static class PermissionCatalog
{
    public static IReadOnlyList<PermissionDefinition> All { get; } =
    [
        new(PermissionKeys.CanLogin, "เข้าสู่ระบบ", PermissionGroups.Account, false, 1),
        new(PermissionKeys.MenuHome, "เมนูหน้าหลัก", PermissionGroups.MainNav, false, 2),
        new(PermissionKeys.MenuPayment, "การชำระเงิน", PermissionGroups.MainNav, false, 3),
        new(PermissionKeys.MenuPackages, "แพ็กเกจ", PermissionGroups.MainNav, false, 4),
        new(PermissionKeys.MenuVisitor, "ผู้มาเยือน", PermissionGroups.MainNav, false, 5),
        new(PermissionKeys.Tickets, "แจ้งปัญหา", PermissionGroups.MainNav, true, 6),
        new(PermissionKeys.MasterHouses, "ข้อมูลหลัก — บ้าน", PermissionGroups.Master, false, 7),
        new(PermissionKeys.MasterUsers, "ข้อมูลหลัก — ผู้ใช้", PermissionGroups.Master, false, 8),
        new(PermissionKeys.MasterRoles, "ข้อมูลหลัก — บทบาทผู้ใช้", PermissionGroups.Master, false, 9),
    ];

    public static PermissionDefinition? Find(string key) =>
        All.FirstOrDefault(p => string.Equals(p.Key, key, StringComparison.Ordinal));
}
