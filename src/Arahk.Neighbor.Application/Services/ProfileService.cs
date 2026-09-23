using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Services;
using Arahk.Neighbor.Domain.ValueObjects;

namespace Arahk.Neighbor.Application.Services;

public class ProfileService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public ProfileService(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<Result<ProfileDto>> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result<ProfileDto>.Fail(ErrorKeys.ServerGeneric);

        return Result<ProfileDto>.Ok(ToDto(user));
    }

    public async Task<Result<ProfileDto>> UpdateAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken ct = default)
    {
        var fieldErrors = ValidateProfile(request);
        if (fieldErrors.Count > 0)
            return Result<ProfileDto>.FailFields(fieldErrors);

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result<ProfileDto>.Fail(ErrorKeys.ServerGeneric);

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            PhoneNumber.TryParse(request.Phone, out phone);
        }

        user.UpdateProfile(request.DisplayName.Trim(), phone);
        await _users.UpdateAsync(user, ct);
        return Result<ProfileDto>.Ok(ToDto(user));
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken ct = default)
    {
        var fieldErrors = ValidateChangePassword(request);
        if (fieldErrors.Count > 0)
            return Result.FailFields(fieldErrors);

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Fail(ErrorKeys.ServerGeneric);

        if (!_hasher.Verify(request.CurrentPassword, user.PasswordHash))
            return Result.FailFields(new Dictionary<string, string>
            {
                ["currentPassword"] = ErrorKeys.PasswordCurrentInvalid
            });

        user.SetPasswordHash(_hasher.Hash(request.NewPassword));
        await _users.UpdateAsync(user, ct);
        return Result.Ok();
    }

    private static ProfileDto ToDto(Domain.Entities.User user) =>
        new(user.Id, user.DisplayName, user.Email, user.PhoneLocal);

    private static Dictionary<string, string> ValidateProfile(UpdateProfileRequest request)
    {
        var errors = new Dictionary<string, string>();
        var nameKey = AuthFieldRules.ValidateDisplayName(request.DisplayName);
        if (nameKey is not null)
            errors["displayName"] = nameKey;

        var phoneKey = AuthFieldRules.ValidatePhone(request.Phone, required: false);
        if (phoneKey is not null)
            errors["phone"] = phoneKey;

        return errors;
    }

    private static Dictionary<string, string> ValidateChangePassword(ChangePasswordRequest request)
    {
        var errors = new Dictionary<string, string>();
        var current = AuthFieldRules.ValidateCurrentPassword(request.CurrentPassword);
        if (current is not null)
            errors["currentPassword"] = current;

        var neu = AuthFieldRules.ValidateNewPassword(request.NewPassword);
        if (neu is not null)
            errors["newPassword"] = neu;

        var confirm = AuthFieldRules.ValidatePasswordConfirm(request.NewPassword, request.NewPasswordConfirm);
        if (confirm is not null)
            errors["newPasswordConfirm"] = confirm;

        return errors;
    }
}
