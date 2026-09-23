using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Arahk.Neighbor.Application.Tests;

public class ProfileServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly DateTimeOffset _now = new(2026, 9, 23, 10, 0, 0, TimeSpan.FromHours(7));

    private ProfileService Sut() => new(_users.Object, _hasher.Object);

    private User MakeUser(string name = "สมชาย ใจดี", string? phone = "0812345678")
    {
        EmailAddress.TryParse("you@email.com", out var em);
        PhoneNumber? ph = null;
        if (phone is not null)
            PhoneNumber.TryParse(phone, out ph);
        var u = User.Create(name, em!, "HASH", true, _now, ph);
        u.MarkEmailVerified();
        return u;
    }

    [Fact]
    public async Task Get_ReturnsProfile()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var result = await Sut().GetAsync(user.Id);
        result.Succeeded.Should().BeTrue();
        result.Data!.DisplayName.Should().Be("สมชาย ใจดี");
        result.Data.Email.Should().Be("you@email.com");
        result.Data.PhoneLocal.Should().Be("0812345678");
    }

    [Fact]
    public async Task Update_ValidatesDisplayName()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var result = await Sut().UpdateAsync(user.Id, new UpdateProfileRequest("A", null));
        result.Succeeded.Should().BeFalse();
        result.FieldErrors["displayName"].Should().Be(ErrorKeys.DisplayNameMin);
    }

    [Fact]
    public async Task Update_ValidatesPhoneWhenProvided()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var result = await Sut().UpdateAsync(user.Id, new UpdateProfileRequest("สมหญิง รักดี", "021234567"));
        result.Succeeded.Should().BeFalse();
        result.FieldErrors["phone"].Should().Be(ErrorKeys.PhoneFormat);
    }

    [Fact]
    public async Task Update_AllowsClearingPhone()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _users.Setup(u => u.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await Sut().UpdateAsync(user.Id, new UpdateProfileRequest("สมหญิง รักดี", "  "));
        result.Succeeded.Should().BeTrue();
        result.Data!.DisplayName.Should().Be("สมหญิง รักดี");
        result.Data.PhoneLocal.Should().BeNull();
        user.PhoneLocal.Should().BeNull();
    }

    [Fact]
    public async Task Update_Success_UpdatesNameAndPhone()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _users.Setup(u => u.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await Sut().UpdateAsync(user.Id, new UpdateProfileRequest("สมหญิง รักดี", "081-999-8877"));
        result.Succeeded.Should().BeTrue();
        result.Data!.DisplayName.Should().Be("สมหญิง รักดี");
        result.Data.PhoneLocal.Should().Be("0819998877");
    }

    [Fact]
    public async Task ChangePassword_WrongCurrent_FieldError()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("wrong-pwd", "HASH")).Returns(false);

        var result = await Sut().ChangePasswordAsync(
            user.Id,
            new ChangePasswordRequest("wrong-pwd", "NewPass99", "NewPass99"));

        result.Succeeded.Should().BeFalse();
        result.FieldErrors["currentPassword"].Should().Be(ErrorKeys.PasswordCurrentInvalid);
        _hasher.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_Success_HashesAndPersists()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("OldPass12", "HASH")).Returns(true);
        _hasher.Setup(h => h.Hash("NewPass99")).Returns("NEW_HASH");
        _users.Setup(u => u.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await Sut().ChangePasswordAsync(
            user.Id,
            new ChangePasswordRequest("OldPass12", "NewPass99", "NewPass99"));

        result.Succeeded.Should().BeTrue();
        user.PasswordHash.Should().Be("NEW_HASH");
    }

    [Fact]
    public async Task ChangePassword_ValidatesComplexity()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await Sut().ChangePasswordAsync(
            user.Id,
            new ChangePasswordRequest("OldPass12", "short", "short"));

        result.Succeeded.Should().BeFalse();
        result.FieldErrors.Should().ContainKey("newPassword");
    }

    [Fact]
    public async Task ChangePassword_ConfirmMismatch()
    {
        var result = await Sut().ChangePasswordAsync(
            Guid.NewGuid(),
            new ChangePasswordRequest("OldPass12", "NewPass99", "NewPass98"));

        result.Succeeded.Should().BeFalse();
        result.FieldErrors["newPasswordConfirm"].Should().Be(ErrorKeys.PasswordConfirmMismatch);
    }
}
