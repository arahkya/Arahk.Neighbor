namespace Arahk.Neighbor.Application.Models;

public sealed record RoleDto(string Key, string NameTh, string NameEn, int SortOrder);

public sealed record PermissionCatalogItemDto(
    string Key,
    string NameTh,
    string Group,
    bool IsProposed,
    bool IsEnabledInSystem,
    IReadOnlyList<string> RoleKeysUsing,
    int SortOrder);

public sealed record PermissionCatalogSummaryDto(
    IReadOnlyList<PermissionCatalogItemDto> Items,
    int TotalCount,
    int EnabledCount,
    int DisabledCount);

public sealed record RolePermissionRowDto(
    string Key,
    string NameTh,
    string Group,
    bool IsProposed,
    bool IsMasterEnabled,
    bool IsGranted,
    bool IsLocked,
    int SortOrder);

public sealed record RolePermissionDetailDto(
    RoleDto Role,
    IReadOnlyList<RolePermissionRowDto> Permissions);

public sealed record SaveRolePermissionsRequest(
    string RoleKey,
    IReadOnlyDictionary<string, bool> Grants);
