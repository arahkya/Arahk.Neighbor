namespace Arahk.Neighbor.Web.Services;

/// <summary>
/// Scoped UI shell state for post-login layout: soft "coming soon" for nav items without pages yet.
/// Soft state does not navigate away from Home; active nav stays on หน้าหลัก.
/// </summary>
public sealed class AppShellState
{
    public string? SoftSoonNavLabel { get; private set; }

    public bool HasSoftSoon => !string.IsNullOrEmpty(SoftSoonNavLabel);

    public event Action? Changed;

    public void ShowComingSoon(string navLabel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(navLabel);
        SoftSoonNavLabel = navLabel.Trim();
        Changed?.Invoke();
    }

    public void ClearComingSoon()
    {
        if (SoftSoonNavLabel is null)
            return;
        SoftSoonNavLabel = null;
        Changed?.Invoke();
    }
}
