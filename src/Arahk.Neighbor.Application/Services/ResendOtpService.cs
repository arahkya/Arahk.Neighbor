using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.ValueObjects;

namespace Arahk.Neighbor.Application.Services;

public class ResendOtpService
{
    private readonly IUserRepository _users;
    private readonly IOtpRepository _otps;
    private readonly IOtpGenerator _otpGenerator;
    private readonly IEmailSender _emailSender;
    private readonly IClock _clock;

    public ResendOtpService(
        IUserRepository users,
        IOtpRepository otps,
        IOtpGenerator otpGenerator,
        IEmailSender emailSender,
        IClock clock)
    {
        _users = users;
        _otps = otps;
        _otpGenerator = otpGenerator;
        _emailSender = emailSender;
        _clock = clock;
    }

    public async Task<Result<OtpSessionInfo>> ExecuteAsync(ResendOtpRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<OtpSessionInfo>.Fail(ErrorKeys.ServerGeneric);

        var now = _clock.UtcNow;
        var existing = await _otps.GetActiveByUserIdAsync(request.UserId, ct);
        if (existing is not null && !existing.CanResend(now))
        {
            return Result<OtpSessionInfo>.Fail(
                ErrorKeys.OtpCooldown,
                cooldownSeconds: existing.CooldownSecondsRemaining(now));
        }

        await _otps.InvalidateActiveForUserAsync(request.UserId, ct);
        var code = _otpGenerator.GenerateSixDigitCode();
        var otp = EmailOtp.Create(user.Id, user.Email, code, now);
        await _otps.AddAsync(otp, ct);
        await _emailSender.SendOtpAsync(user.Email, code, ct);

        var masked = EmailAddress.TryParse(user.Email, out var em) ? em!.ToMasked() : user.Email;

        return Result<OtpSessionInfo>.Ok(new OtpSessionInfo(
            user.Id,
            user.Email,
            masked,
            otp.CooldownSecondsRemaining(now),
            otp.RemainingAttempts()));
    }
}
