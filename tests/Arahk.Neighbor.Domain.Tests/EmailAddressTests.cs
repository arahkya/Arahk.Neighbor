using Arahk.Neighbor.Domain.ValueObjects;
using FluentAssertions;

namespace Arahk.Neighbor.Domain.Tests;

public class EmailAddressTests
{
    [Theory]
    [InlineData("You@Email.com", "you@email.com")]
    [InlineData("  a@b.co  ", "a@b.co")]
    public void TryParse_NormalizesToLowerTrimmed(string input, string expected)
    {
        EmailAddress.TryParse(input, out var email).Should().BeTrue();
        email!.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("a@b")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParse_RejectsInvalid(string? input)
    {
        EmailAddress.TryParse(input, out _).Should().BeFalse();
    }

    [Fact]
    public void ToMasked_HidesLocalPart()
    {
        EmailAddress.TryParse("you@email.com", out var email);
        email!.ToMasked().Should().Be("yo***@email.com");
    }
}
