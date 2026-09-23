namespace Arahk.Neighbor.Application.Models;

public record RegisterRequest(
    string DisplayName,
    string Email,
    string? Phone,
    string Password,
    string PasswordConfirm,
    bool TermsAccepted);

public record RegisterResponse(Guid UserId, string Email, string MaskedEmail);

public enum LoginMode { Email, Phone }

public record LoginRequest(LoginMode Mode, string Identifier, string Password);

public record LoginResponse(Guid UserId, string DisplayName, string Email, bool EmailVerified);

public record VerifyOtpRequest(Guid UserId, string Code);

public record ResendOtpRequest(Guid UserId);

public record OtpSessionInfo(
    Guid UserId,
    string Email,
    string MaskedEmail,
    int CooldownSecondsRemaining,
    int RemainingAttempts);
