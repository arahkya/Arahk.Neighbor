namespace Arahk.Neighbor.Domain.Constants;

public static class AuthConstants
{
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;
    public const int DisplayNameMinLength = 2;
    public const int DisplayNameMaxLength = 50;
    public const int EmailMaxLength = 254;
    public const int OtpLength = 6;
    public const int OtpExpirySeconds = 600; // 10 minutes
    public const int OtpMaxAttempts = 5;
    public const int OtpResendCooldownSeconds = 60;
}
