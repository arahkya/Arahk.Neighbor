using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.Services;
using FluentAssertions;

namespace Arahk.Neighbor.Domain.Tests;

public class HouseFieldRulesTests
{
    [Fact]
    public void HouseNo_required_after_trim()
    {
        HouseFieldRules.ValidateHouseNo(null).Should().Be(ErrorKeys.HouseNoRequired);
        HouseFieldRules.ValidateHouseNo("   ").Should().Be(ErrorKeys.HouseNoRequired);
    }

    [Fact]
    public void HouseNo_max_length()
    {
        HouseFieldRules.ValidateHouseNo(new string('1', 33)).Should().Be(ErrorKeys.HouseNoTooLong);
        HouseFieldRules.ValidateHouseNo(new string('1', 32)).Should().BeNull();
    }

    [Fact]
    public void HouseNo_trims_before_validate()
    {
        HouseFieldRules.ValidateHouseNo("  12/1  ").Should().BeNull();
        House.NormalizeHouseNo("  12/1  ").Should().Be("12/1");
    }

    [Fact]
    public void HouseNo_equals_exact_after_trim_no_casefold()
    {
        HouseFieldRules.HouseNoEquals(" 12/1 ", "12/1").Should().BeTrue();
        HouseFieldRules.HouseNoEquals("12/1", "12 /1").Should().BeFalse();
        HouseFieldRules.HouseNoEquals("Ab", "ab").Should().BeFalse();
    }

    [Fact]
    public void Soi_optional_and_max()
    {
        HouseFieldRules.ValidateSoi(null).Should().BeNull();
        HouseFieldRules.ValidateSoi("  ").Should().BeNull();
        House.NormalizeSoi("  ").Should().BeNull();
        HouseFieldRules.ValidateSoi(new string('x', 101)).Should().Be(ErrorKeys.SoiTooLong);
        HouseFieldRules.ValidateSoi("ซอยร่มเงา").Should().BeNull();
    }

    [Fact]
    public void House_Create_and_Update_normalize()
    {
        var now = DateTimeOffset.Parse("2026-09-23T09:00:00Z");
        var house = House.Create(HouseConstants.DefaultCommunityId, "  12/1  ", "  ซอย  ", now);
        house.HouseNo.Should().Be("12/1");
        house.Soi.Should().Be("ซอย");

        house.Update("12/2", "   ", now);
        house.HouseNo.Should().Be("12/2");
        house.Soi.Should().BeNull();
    }
}
