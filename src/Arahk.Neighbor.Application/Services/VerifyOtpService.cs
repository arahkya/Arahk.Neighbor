using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.Services;

namespace Arahk.Neighbor.Application.Services;

public class VerifyOtpService
{
    private readonly IUserRepository _users;
    private readonly IOtpRepository _otps;
    private readonly IClock _clock;

    public VerifyOtpService(IUserRepository users, IOtpRepository otps, IClock clock)
    {
        _users = users;
        _otps = otps;
        _clock = clock;
    }

    public async Task<Result<LoginResponse>> ExecuteAsync(VerifyOtpRequest request, CancellationToken ct = default)
    {
        var format = AuthFieldRules.ValidateOtpFormat(request.Code);
        if (format is not null)
            return Result<LoginResponse>.Fail(format);

        var user = await _users.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<LoginResponse>.Fail(ErrorKeys.ServerGeneric);

        var otp = await _otps.GetActiveByUserIdAsync(request.UserId, ct);
        if (otp is null)
            return Result<LoginResponse>.Fail(ErrorKeys.OtpExpired);

        var now = _clock.UtcNow;
        var outcome = otp.Verify(request.Code.Trim(), now);
        await _otps.UpdateAsync(otp, ct);

        return outcome switch
        {
            OtpVerifyOutcome.Success => await OnSuccessAsync(user, ct),
            OtpVerifyOutcome.Expired => Result<LoginResponse>.Fail(ErrorKeys.OtpExpired),
            OtpVerifyOutcome.MaxAttempts => Result<LoginResponse>.Fail(ErrorKeys.OtpMaxAttempts),
            OtpVerifyOutcome.Invalid => Result<LoginResponse>.Fail(
                ErrorKeys.OtpInvalidRemaining, remainingAttempts: otp.RemainingAttempts()),
            _ => Result<LoginResponse>.Fail(ErrorKeys.ServerGeneric)
        };
    }

    private async Task<Result<LoginResponse>> OnSuccessAsync(User user, CancellationToken ct)
    {
        user.MarkEmailVerified();
        await _users.UpdateAsync(user, ct);
        return Result<LoginResponse>.Ok(new LoginResponse(user.Id, user.DisplayName, user.Email, true));
    }
}
