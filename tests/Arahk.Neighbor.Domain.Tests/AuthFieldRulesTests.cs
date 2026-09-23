using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Services;
using FluentAssertions;

namespace Arahk.Neighbor.Domain.Tests;

public class AuthFieldRulesTests
{
    [Fact]
    public void DisplayName_Required()
    {
        AuthFieldRules.ValidateDisplayName(" ").Should().Be(ErrorKeys.DisplayNameRequired);
    }

    [Fact]
    public void DisplayName_MinMax()
    {
        AuthFieldRules.ValidateDisplayName("A").Should().Be(ErrorKeys.DisplayNameMin);
        AuthFieldRules.ValidateDisplayName(new string('x', 51)).Should().Be(ErrorKeys.DisplayNameMax);
        AuthFieldRules.ValidateDisplayName("มานี").Should().BeNull();
    }

    [Fact]
    public void Password_Register_Complexity()
    {
        AuthFieldRules.ValidatePasswordForRegister("short1").Should().Be(ErrorKeys.PasswordMin);
        AuthFieldRules.ValidatePasswordForRegister("onlyletters").Should().Be(ErrorKeys.PasswordComplexity);
        AuthFieldRules.ValidatePasswordForRegister("12345678").Should().Be(ErrorKeys.PasswordComplexity);
        AuthFieldRules.ValidatePasswordForRegister("Pass1234").Should().BeNull();
    }

    [Fact]
    public void PasswordConfirm_Mismatch()
    {
        AuthFieldRules.ValidatePasswordConfirm("Pass1234", "Pass1235")
            .Should().Be(ErrorKeys.PasswordConfirmMismatch);
    }

    [Fact]
    public void Terms_Required()
    {
        AuthFieldRules.ValidateTerms(false).Should().Be(ErrorKeys.TermsRequired);
        AuthFieldRules.ValidateTerms(true).Should().BeNull();
    }

    [Fact]
    public void Otp_Format()
    {
        AuthFieldRules.ValidateOtpFormat("").Should().Be(ErrorKeys.OtpRequired);
        AuthFieldRules.ValidateOtpFormat("12345").Should().Be(ErrorKeys.OtpFormat);
        AuthFieldRules.ValidateOtpFormat("abcdef").Should().Be(ErrorKeys.OtpFormat);
        AuthFieldRules.ValidateOtpFormat("123456").Should().BeNull();
    }

    [Fact]
    public void Phone_OptionalWhenEmpty()
    {
        AuthFieldRules.ValidatePhone(null, required: false).Should().BeNull();
        AuthFieldRules.ValidatePhone("0812345678", required: false).Should().BeNull();
        AuthFieldRules.ValidatePhone("bad", required: false).Should().Be(ErrorKeys.PhoneFormat);
    }

    [Fact]
    public void CurrentAndNewPassword_Rules()
    {
        AuthFieldRules.ValidateCurrentPassword("").Should().Be(ErrorKeys.PasswordCurrentRequired);
        AuthFieldRules.ValidateCurrentPassword("x").Should().BeNull();
        AuthFieldRules.ValidateNewPassword("").Should().Be(ErrorKeys.PasswordNewRequired);
        AuthFieldRules.ValidateNewPassword("short1").Should().Be(ErrorKeys.PasswordMin);
        AuthFieldRules.ValidateNewPassword("onlyletters").Should().Be(ErrorKeys.PasswordComplexity);
        AuthFieldRules.ValidateNewPassword("Pass1234").Should().BeNull();
    }
}
