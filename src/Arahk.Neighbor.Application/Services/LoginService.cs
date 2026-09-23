using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.Services;
using Arahk.Neighbor.Domain.ValueObjects;

namespace Arahk.Neighbor.Application.Services;

public class LoginService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public LoginService(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<Result<LoginResponse>> ExecuteAsync(LoginRequest request, CancellationToken ct = default)
    {
        var fieldErrors = Validate(request);
        if (fieldErrors.Count > 0)
            return Result<LoginResponse>.FailFields(fieldErrors);

        User? user;
        string credentialsKey;

        if (request.Mode == LoginMode.Email)
        {
            credentialsKey = ErrorKeys.LoginCredentials;
            EmailAddress.TryParse(request.Identifier, out var email);
            user = await _users.GetByEmailAsync(email!.Value, ct);
        }
        else
        {
            credentialsKey = ErrorKeys.LoginCredentialsPhone;
            PhoneNumber.TryParse(request.Identifier, out var phone);
            user = await _users.GetByPhoneLocalAsync(phone!.LocalForm, ct);
            if (user is null)
                return Result<LoginResponse>.Fail(ErrorKeys.LoginPhoneNotLinked);
        }

        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Fail(credentialsKey);

        return Result<LoginResponse>.Ok(new LoginResponse(user.Id, user.DisplayName, user.Email, user.EmailVerified));
    }

    private static Dictionary<string, string> Validate(LoginRequest request)
    {
        var errors = new Dictionary<string, string>();
        if (request.Mode == LoginMode.Email)
        {
            var key = AuthFieldRules.ValidateEmail(request.Identifier);
            if (key is not null) errors["identifier"] = key;
        }
        else
        {
            var key = AuthFieldRules.ValidatePhone(request.Identifier, required: true);
            if (key is not null) errors["identifier"] = key;
        }

        var pwd = AuthFieldRules.ValidatePasswordRequired(request.Password);
        if (pwd is not null) errors["password"] = pwd;
        return errors;
    }
}
