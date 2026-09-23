namespace Arahk.Neighbor.Domain.Entities;

/// <summary>System-wide master toggle for one permission in a community.</summary>
public class PermissionMasterState
{
    public Guid CommunityId { get; private set; }
    public string PermissionKey { get; private set; } = string.Empty;
    public bool IsEnabledInSystem { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private PermissionMasterState() { }

    public static PermissionMasterState Create(
        Guid communityId,
        string permissionKey,
        bool enabled,
        DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionKey);
        return new PermissionMasterState
        {
            CommunityId = communityId,
            PermissionKey = permissionKey.Trim(),
            IsEnabledInSystem = enabled,
            UpdatedAt = now
        };
    }

    public void SetEnabled(bool enabled, DateTimeOffset now)
    {
        IsEnabledInSystem = enabled;
        UpdatedAt = now;
    }
}
