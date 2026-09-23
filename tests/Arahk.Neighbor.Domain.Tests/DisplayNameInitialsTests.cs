using Arahk.Neighbor.Domain.Services;
using FluentAssertions;

namespace Arahk.Neighbor.Domain.Tests;

public class DisplayNameInitialsTests
{
    [Theory]
    [InlineData("สมชาย ใจดี", "สจ")]
    [InlineData("สมหญิง รักดี", "สร")]
    [InlineData("อารักษ์", "อา")]
    [InlineData("A", "A")]
    [InlineData("Alice Bob", "AB")]
    [InlineData("  ", "?")]
    [InlineData(null, "?")]
    public void FromDisplayName_MatchesUxRule(string? name, string expected)
    {
        DisplayNameInitials.FromDisplayName(name).Should().Be(expected);
    }
}
