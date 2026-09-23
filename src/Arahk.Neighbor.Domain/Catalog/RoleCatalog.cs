using Arahk.Neighbor.Domain.Constants;

namespace Arahk.Neighbor.Domain.Catalog;

public sealed record RoleDefinition(
    string Key,
    string NameTh,
    string NameEn,
    int SortOrder);

/// <summary>Static five community roles — no Internal Workers.</summary>
public static class RoleCatalog
{
    public static IReadOnlyList<RoleDefinition> All { get; } =
    [
        new(RoleKeys.Resident, "ลูกบ้าน", "Resident", 1),
        new(RoleKeys.Juristic, "นิติบุคคล", "Juristic", 2),
        new(RoleKeys.Committee, "คณะกรรมการหมู่บ้าน", "Committee", 3),
        new(RoleKeys.Auditor, "ผู้ตรวจสอบบัญชีภายนอก", "Auditor", 4),
        new(RoleKeys.Security, "เจ้าหน้าที่รปภ.", "Security", 5),
    ];

    public static RoleDefinition? Find(string key) =>
        All.FirstOrDefault(r => string.Equals(r.Key, key, StringComparison.Ordinal));
}
