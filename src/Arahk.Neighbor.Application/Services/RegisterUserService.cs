using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.Services;
using Arahk.Neighbor.Domain.ValueObjects;

namespace Arahk.Neighbor.Application.Services;

public class RegisterUserService
{
    private readonly IUserRepository _users;
    private readonly IOtpRepository _otps;
    private readonly IPasswordHasher _hasher;
    private readonly IOtpGenerator _otpGenerator;
    private readonly IEmailSender _emailSender;
    private readonly IClock _clock;

    public RegisterUserService(
        IUserRepository users,
        IOtpRepository otps,
        IPasswordHasher hasher,
        IOtpGenerator otpGenerator,
        IEmailSender emailSender,
        IClock clock)
    {
        _users = users;
        _otps = otps;
        _hasher = hasher;
        _otpGenerator = otpGenerator;
        _emailSender = emailSender;
        _clock = clock;
    }

    public async Task<Result<RegisterResponse>> ExecuteAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var fieldErrors = Validate(request);
        if (fieldErrors.Count > 0)
            return Result<RegisterResponse>.FailFields(fieldErrors);

        EmailAddress.TryParse(request.Email, out var email);
        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(request.Phone))
            PhoneNumber.TryParse(request.Phone, out phone);

        var now = _clock.UtcNow;
        var existing = await _users.GetByEmailAsync(email!.Value, ct);

        if (existing is not null && existing.EmailVerified)
            return Result<RegisterResponse>.Fail(ErrorKeys.EmailDuplicate);

        var hash = _hasher.Hash(request.Password);
        User user;

        if (existing is not null && !existing.EmailVerified)
        {
            existing.UpdatePendingRegistration(request.DisplayName, hash, phone, now);
            await _users.UpdateAsync(existing, ct);
            user = existing;
        }
        else
        {
            user = User.Create(request.DisplayName, email, hash, request.TermsAccepted, now, phone);
            await _users.AddAsync(user, ct);
        }

        await _otps.InvalidateActiveForUserAsync(user.Id, ct);
        var code = _otpGenerator.GenerateSixDigitCode();
        var otp = EmailOtp.Create(user.Id, user.Email, code, now);
        await _otps.AddAsync(otp, ct);
        await _emailSender.SendOtpAsync(user.Email, code, ct);

        return Result<RegisterResponse>.Ok(new RegisterResponse(user.Id, user.Email, email.ToMasked()));
    }

    private static Dictionary<string, string> Validate(RegisterRequest request)
    {
        var errors = new Dictionary<string, string>();
        Add(errors, "displayName", AuthFieldRules.ValidateDisplayName(request.DisplayName));
        Add(errors, "email", AuthFieldRules.ValidateEmail(request.Email));
        Add(errors, "phone", AuthFieldRules.ValidatePhone(request.Phone, required: false));
        Add(errors, "password", AuthFieldRules.ValidatePasswordForRegister(request.Password));
        Add(errors, "passwordConfirm", AuthFieldRules.ValidatePasswordConfirm(request.Password, request.PasswordConfirm));
        Add(errors, "terms", AuthFieldRules.ValidateTerms(request.TermsAccepted));
        return errors;
    }

    private static void Add(Dictionary<string, string> dict, string field, string? key)
    {
        if (key is not null) dict[field] = key;
    }
}
