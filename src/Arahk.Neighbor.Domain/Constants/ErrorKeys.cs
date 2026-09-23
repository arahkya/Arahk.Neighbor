namespace Arahk.Neighbor.Domain.Constants;

/// <summary>Error / copy keys aligned with UX 03-copy-th.md</summary>
public static class ErrorKeys
{
    public const string EmailRequired = "err.email.required";
    public const string EmailFormat = "err.email.format";
    public const string EmailDuplicate = "err.email.duplicate";
    public const string PhoneRequired = "err.phone.required";
    public const string PhoneFormat = "err.phone.format";
    public const string PasswordRequired = "err.password.required";
    public const string PasswordMin = "err.password.min";
    public const string PasswordComplexity = "err.password.complexity";
    public const string PasswordConfirmRequired = "err.password_confirm.required";
    public const string PasswordConfirmMismatch = "err.password_confirm.mismatch";
    public const string DisplayNameRequired = "err.displayname.required";
    public const string DisplayNameMin = "err.displayname.min";
    public const string DisplayNameMax = "err.displayname.max";
    public const string TermsRequired = "err.terms.required";
    public const string LoginCredentials = "err.login.credentials";
    public const string LoginCredentialsPhone = "err.login.credentials_phone";
    public const string LoginPhoneNotLinked = "err.login.phone_not_linked";
    public const string OtpRequired = "err.otp.required";
    public const string OtpFormat = "err.otp.format";
    public const string OtpInvalid = "err.otp.invalid";
    public const string OtpInvalidRemaining = "err.otp.invalid_remaining";
    public const string OtpExpired = "err.otp.expired";
    public const string OtpMaxAttempts = "err.otp.max_attempts";
    public const string OtpCooldown = "err.otp.cooldown";
    public const string NetworkGeneric = "error.network_generic";
    public const string ServerGeneric = "error.server_generic";
}
