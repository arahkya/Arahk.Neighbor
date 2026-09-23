using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Arahk.Neighbor.Application.Tests;

public class RegisterUserServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IOtpRepository> _otps = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IOtpGenerator> _otpGen = new();
    private readonly Mock<IEmailSender> _email = new();
    private readonly Mock<IClock> _clock = new();
    private readonly DateTimeOffset _now = new(2026, 9, 23, 10, 0, 0, TimeSpan.FromHours(7));

    private RegisterUserService Sut()
    {
        _clock.Setup(c => c.UtcNow).Returns(_now);
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("HASH");
        _otpGen.Setup(g => g.GenerateSixDigitCode()).Returns("654321");
        return new RegisterUserService(_users.Object, _otps.Object, _hasher.Object, _otpGen.Object, _email.Object, _clock.Object);
    }

    private static RegisterRequest Valid() =>
        new("มานี", "you@email.com", "0812345678", "Pass1234", "Pass1234", true);

    [Fact]
    public async Task Register_HappyPath_CreatesUserSendsOtp()
    {
        _users.Setup(u => u.GetByEmailAsync("you@email.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var result = await Sut().ExecuteAsync(Valid());
        result.Succeeded.Should().BeTrue();
        result.Data!.Email.Should().Be("you@email.com");
        result.Data.MaskedEmail.Should().Be("yo***@email.com");
        _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _otps.Verify(o => o.AddAsync(It.IsAny<EmailOtp>(), It.IsAny<CancellationToken>()), Times.Once);
        _email.Verify(e => e.SendOtpAsync("you@email.com", "654321", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_DuplicateVerifiedEmail_Fails()
    {
        EmailAddress.TryParse("you@email.com", out var em);
        var existing = User.Create("Old", em!, "H", true, _now);
        existing.MarkEmailVerified();
        _users.Setup(u => u.GetByEmailAsync("you@email.com", It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        var result = await Sut().ExecuteAsync(Valid());
        result.Succeeded.Should().BeFalse();
        result.ErrorKey.Should().Be(ErrorKeys.EmailDuplicate);
    }

    [Fact]
    public async Task Register_ValidationErrors_ReturnedAsFieldErrors()
    {
        var result = await Sut().ExecuteAsync(new RegisterRequest("", "bad", null, "x", "y", false));
        result.Succeeded.Should().BeFalse();
        result.FieldErrors.Should().ContainKey("displayName");
        result.FieldErrors.Should().ContainKey("email");
        result.FieldErrors.Should().ContainKey("password");
        result.FieldErrors.Should().ContainKey("terms");
    }

    [Fact]
    public async Task Register_OptionalPhoneOmitted_Ok()
    {
        _users.Setup(u => u.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var result = await Sut().ExecuteAsync(Valid() with { Phone = null });
        result.Succeeded.Should().BeTrue();
    }
}
