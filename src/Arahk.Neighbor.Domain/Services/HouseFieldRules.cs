using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Domain.Services;

/// <summary>Pure validation for HouseNo / Soi (UX 04-validation-rules.md).</summary>
public static class HouseFieldRules
{
    public static string? ValidateHouseNo(string? houseNo)
    {
        var trimmed = House.NormalizeHouseNo(houseNo);
        if (string.IsNullOrEmpty(trimmed))
            return ErrorKeys.HouseNoRequired;
        if (trimmed.Length > HouseConstants.HouseNoMaxLength)
            return ErrorKeys.HouseNoTooLong;
        return null;
    }

    public static string? ValidateSoi(string? soi)
    {
        var normalized = House.NormalizeSoi(soi);
        if (normalized is not null && normalized.Length > HouseConstants.SoiMaxLength)
            return ErrorKeys.SoiTooLong;
        return null;
    }

    /// <summary>Exact string compare after trim — no case-fold, no mid-space collapse.</summary>
    public static bool HouseNoEquals(string? a, string? b) =>
        string.Equals(House.NormalizeHouseNo(a), House.NormalizeHouseNo(b), StringComparison.Ordinal);
}
