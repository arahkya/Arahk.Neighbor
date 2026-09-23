namespace Arahk.Neighbor.Web.Services;

public class AuthSessionState
{
    public bool IsAuthenticated { get; private set; }
    public Guid? UserId { get; private set; }
    public string? DisplayName { get; private set; }
    public string? Email { get; private set; }

    public Guid? PendingOtpUserId { get; private set; }
    public string? PendingOtpEmail { get; private set; }
    public string? PendingOtpMaskedEmail { get; private set; }

    public event Action? Changed;

    public void SignIn(Guid userId, string displayName, string email)
    {
        IsAuthenticated = true;
        UserId = userId;
        DisplayName = displayName;
        Email = email;
        ClearPendingOtp();
        Changed?.Invoke();
    }

    public void SignOut()
    {
        IsAuthenticated = false;
        UserId = null;
        DisplayName = null;
        Email = null;
        ClearPendingOtp();
        Changed?.Invoke();
    }

    /// <summary>Refresh display name after profile save (AppBar initials + welcome).</summary>
    public void UpdateDisplayName(string displayName)
    {
        if (!IsAuthenticated)
            return;
        DisplayName = displayName;
        Changed?.Invoke();
    }

    public void SetPendingOtp(Guid userId, string email, string maskedEmail)
    {
        PendingOtpUserId = userId;
        PendingOtpEmail = email;
        PendingOtpMaskedEmail = maskedEmail;
        Changed?.Invoke();
    }

    public void ClearPendingOtp()
    {
        PendingOtpUserId = null;
        PendingOtpEmail = null;
        PendingOtpMaskedEmail = null;
    }
}
