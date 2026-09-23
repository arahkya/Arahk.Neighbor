using Arahk.Neighbor.Web.Services;
using FluentAssertions;

namespace Arahk.Neighbor.Web.Tests;

public class AppShellStateTests
{
    [Fact]
    public void ShowComingSoon_sets_label_and_raises_changed()
    {
        var sut = new AppShellState();
        var raised = 0;
        sut.Changed += () => raised++;

        sut.ShowComingSoon("การชำระเงิน");

        sut.HasSoftSoon.Should().BeTrue();
        sut.SoftSoonNavLabel.Should().Be("การชำระเงิน");
        raised.Should().Be(1);
    }

    [Fact]
    public void ClearComingSoon_clears_label()
    {
        var sut = new AppShellState();
        sut.ShowComingSoon("แพ็กเกจ");
        var raised = 0;
        sut.Changed += () => raised++;

        sut.ClearComingSoon();

        sut.HasSoftSoon.Should().BeFalse();
        sut.SoftSoonNavLabel.Should().BeNull();
        raised.Should().Be(1);
    }

    [Fact]
    public void ClearComingSoon_when_empty_does_not_raise()
    {
        var sut = new AppShellState();
        var raised = 0;
        sut.Changed += () => raised++;

        sut.ClearComingSoon();

        raised.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ShowComingSoon_rejects_blank(string? label)
    {
        var sut = new AppShellState();
        var act = () => sut.ShowComingSoon(label!);
        act.Should().Throw<ArgumentException>();
    }
}
