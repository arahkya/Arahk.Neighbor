namespace Arahk.Neighbor.Domain.Entities;

/// <summary>
/// Per-role grant for a permission. Persists even when master is OFF
/// so re-enabling master restores the previous value.
/// </summary>
public class RolePermissionGrant
{
    public Guid CommunityId { get; private set; }
    public string RoleKey { get; private set; } = string.Empty;
    public string PermissionKey { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private RolePermissionGrant() { }

    public static RolePermissionGrant Create(
        Guid communityId,
        string roleKey,
        string permissionKey,
        bool enabled,
        DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roleKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionKey);
        return new RolePermissionGrant
        {
            CommunityId = communityId,
            RoleKey = roleKey.Trim(),
            PermissionKey = permissionKey.Trim(),
            IsEnabled = enabled,
            UpdatedAt = now
        };
    }

    public void SetEnabled(bool enabled, DateTimeOffset now)
    {
        IsEnabled = enabled;
        UpdatedAt = now;
    }
}
