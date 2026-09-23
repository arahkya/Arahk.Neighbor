namespace Arahk.Neighbor.Application.Services;

/// <summary>
/// In-memory dirty-state editor for a role's permission switches.
/// Explicit Save/Cancel — no auto-save.
/// </summary>
public sealed class RolePermissionDraft
{
    private readonly Dictionary<string, bool> _baseline;
    private readonly Dictionary<string, bool> _current;
    private readonly HashSet<string> _lockedKeys;

    public RolePermissionDraft(
        IReadOnlyDictionary<string, bool> baselineGrants,
        IEnumerable<string>? lockedPermissionKeys = null)
    {
        _baseline = new Dictionary<string, bool>(baselineGrants, StringComparer.Ordinal);
        _current = new Dictionary<string, bool>(_baseline, StringComparer.Ordinal);
        _lockedKeys = new HashSet<string>(
            lockedPermissionKeys ?? Array.Empty<string>(),
            StringComparer.Ordinal);
    }

    public bool IsDirty =>
        _current.Count != _baseline.Count ||
        _current.Any(kv => !_baseline.TryGetValue(kv.Key, out var b) || b != kv.Value);

    public IReadOnlyDictionary<string, bool> Current => _current;

    public bool IsLocked(string permissionKey) => _lockedKeys.Contains(permissionKey);

    public bool TrySet(string permissionKey, bool enabled)
    {
        if (_lockedKeys.Contains(permissionKey))
            return false;
        if (!_current.ContainsKey(permissionKey) && !_baseline.ContainsKey(permissionKey))
            return false;
        _current[permissionKey] = enabled;
        return true;
    }

    public void Cancel()
    {
        _current.Clear();
        foreach (var kv in _baseline)
            _current[kv.Key] = kv.Value;
    }

    public IReadOnlyDictionary<string, bool> SnapshotForSave() =>
        new Dictionary<string, bool>(_current, StringComparer.Ordinal);

    public void MarkSaved()
    {
        _baseline.Clear();
        foreach (var kv in _current)
            _baseline[kv.Key] = kv.Value;
    }
}
