namespace Arahk.Neighbor.Domain.Constants;

public static class HouseConstants
{
    /// <summary>Single-community default until multi-tenant is introduced.</summary>
    public static readonly Guid DefaultCommunityId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public const int HouseNoMaxLength = 32;
    public const int SoiMaxLength = 100;

    public const string ExcelSheetName = "Houses";
    public const string ExcelColumnHouseNo = "HouseNo";
    public const string ExcelColumnSoi = "Soi";
    public const string ExcelExtension = ".xlsx";
    public const long ExcelMaxBytes = 5L * 1024 * 1024;
    public const int ExcelMaxDataRows = 1000;
}
