using Arahk.Neighbor.Domain.Constants;

namespace Arahk.Neighbor.Domain.Entities;

public class EmailOtp
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset ResendAvailableAt { get; private set; }
    public int FailedAttempts { get; private set; }
    public bool IsConsumed { get; private set; }
    public bool IsInvalidated { get; private set; }

    private EmailOtp() { }

    public static EmailOtp Create(Guid userId, string email, string code, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != AuthConstants.OtpLength || !code.All(char.IsDigit))
            throw new ArgumentException("OTP must be 6 digits.", nameof(code));

        return new EmailOtp
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email.Trim().ToLowerInvariant(),
            Code = code,
            CreatedAt = now,
            ExpiresAt = now.AddSeconds(AuthConstants.OtpExpirySeconds),
            ResendAvailableAt = now.AddSeconds(AuthConstants.OtpResendCooldownSeconds),
            FailedAttempts = 0,
            IsConsumed = false,
            IsInvalidated = false
        };
    }

    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    public bool HasExceededAttempts() => FailedAttempts >= AuthConstants.OtpMaxAttempts;

    public bool CanResend(DateTimeOffset now) => now >= ResendAvailableAt;

    public int RemainingAttempts() => Math.Max(0, AuthConstants.OtpMaxAttempts - FailedAttempts);

    public int CooldownSecondsRemaining(DateTimeOffset now)
    {
        var rem = (int)Math.Ceiling((ResendAvailableAt - now).TotalSeconds);
        return Math.Max(0, rem);
    }

    public OtpVerifyOutcome Verify(string inputCode, DateTimeOffset now)
    {
        if (IsConsumed || IsInvalidated)
            return OtpVerifyOutcome.MaxAttempts;

        if (HasExceededAttempts())
            return OtpVerifyOutcome.MaxAttempts;

        if (IsExpired(now))
            return OtpVerifyOutcome.Expired;

        if (Code != inputCode)
        {
            FailedAttempts++;
            if (HasExceededAttempts())
            {
                IsInvalidated = true;
                return OtpVerifyOutcome.MaxAttempts;
            }
            return OtpVerifyOutcome.Invalid;
        }

        IsConsumed = true;
        return OtpVerifyOutcome.Success;
    }

    public void Invalidate()
    {
        IsInvalidated = true;
    }
}

public enum OtpVerifyOutcome
{
    Success,
    Invalid,
    Expired,
    MaxAttempts
}
