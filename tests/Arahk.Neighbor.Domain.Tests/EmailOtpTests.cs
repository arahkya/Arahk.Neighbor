using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using FluentAssertions;

namespace Arahk.Neighbor.Domain.Tests;

public class EmailOtpTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    private static readonly DateTimeOffset T0 = new(2026, 9, 23, 10, 0, 0, TimeSpan.FromHours(7));

    [Fact]
    public void Create_SetsExpiryAndCooldown()
    {
        var otp = EmailOtp.Create(UserId, "a@b.co", "123456", T0);
        otp.ExpiresAt.Should().Be(T0.AddSeconds(AuthConstants.OtpExpirySeconds));
        otp.ResendAvailableAt.Should().Be(T0.AddSeconds(AuthConstants.OtpResendCooldownSeconds));
        otp.RemainingAttempts().Should().Be(5);
    }

    [Fact]
    public void Verify_Success_Consumes()
    {
        var otp = EmailOtp.Create(UserId, "a@b.co", "123456", T0);
        otp.Verify("123456", T0).Should().Be(OtpVerifyOutcome.Success);
        otp.IsConsumed.Should().BeTrue();
    }

    [Fact]
    public void Verify_WrongCode_DecrementsAttempts()
    {
        var otp = EmailOtp.Create(UserId, "a@b.co", "123456", T0);
        otp.Verify("000000", T0).Should().Be(OtpVerifyOutcome.Invalid);
        otp.RemainingAttempts().Should().Be(4);
    }

    [Fact]
    public void Verify_FifthWrong_ReturnsMaxAttempts()
    {
        var otp = EmailOtp.Create(UserId, "a@b.co", "123456", T0);
        for (var i = 0; i < 4; i++)
            otp.Verify("000000", T0).Should().Be(OtpVerifyOutcome.Invalid);

        otp.Verify("000000", T0).Should().Be(OtpVerifyOutcome.MaxAttempts);
        otp.HasExceededAttempts().Should().BeTrue();
        otp.IsInvalidated.Should().BeTrue();
    }

    [Fact]
    public void Verify_Expired()
    {
        var otp = EmailOtp.Create(UserId, "a@b.co", "123456", T0);
        var later = T0.AddMinutes(11);
        otp.Verify("123456", later).Should().Be(OtpVerifyOutcome.Expired);
    }

    [Fact]
    public void CanResend_RespectsCooldown()
    {
        var otp = EmailOtp.Create(UserId, "a@b.co", "123456", T0);
        otp.CanResend(T0.AddSeconds(30)).Should().BeFalse();
        otp.CooldownSecondsRemaining(T0.AddSeconds(30)).Should().Be(30);
        otp.CanResend(T0.AddSeconds(60)).Should().BeTrue();
    }
}
