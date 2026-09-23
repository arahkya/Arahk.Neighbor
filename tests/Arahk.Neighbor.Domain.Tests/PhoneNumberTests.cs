using Arahk.Neighbor.Domain.ValueObjects;
using FluentAssertions;

namespace Arahk.Neighbor.Domain.Tests;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("0812345678")]
    [InlineData("081-234-5678")]
    [InlineData("+66812345678")]
    [InlineData("66 812345678")]
    [InlineData("0912345678")]
    [InlineData("0612345678")]
    public void TryParse_AcceptsValidThaiMobile(string input)
    {
        PhoneNumber.TryParse(input, out var phone).Should().BeTrue();
        phone!.LocalForm.Should().MatchRegex(@"^0[689]\d{8}$");
        phone.E164.Should().StartWith("+66");
        phone.E164.Length.Should().Be(12);
    }

    [Theory]
    [InlineData("021234567")]
    [InlineData("081234567")]
    [InlineData("0212345678")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParse_RejectsInvalid(string? input)
    {
        PhoneNumber.TryParse(input, out var phone).Should().BeFalse();
        phone.Should().BeNull();
    }
}
