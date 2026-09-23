namespace Arahk.Neighbor.Application.Models;

public sealed record HouseDto(Guid Id, string HouseNo, string? Soi);

public sealed record CreateHouseRequest(string? HouseNo, string? Soi);

public sealed record UpdateHouseRequest(Guid Id, string? HouseNo, string? Soi);

public sealed record HouseImportRow(int ExcelRowNumber, string? HouseNo, string? Soi);

public sealed record HouseImportRowIssue(int ExcelRowNumber, string MessageKey);

public sealed class HouseImportResult
{
    public int Ok { get; init; }
    public int Skip { get; init; }
    public int Fail { get; init; }
    public bool FileRejected { get; init; }
    public string? FileErrorKey { get; init; }
    public IReadOnlyList<HouseImportRowIssue> Issues { get; init; } = Array.Empty<HouseImportRowIssue>();

    public bool HasAnyImported => Ok > 0;
    public bool IsFullSuccess => !FileRejected && Ok > 0 && Skip == 0 && Fail == 0;
    public bool IsPartial => !FileRejected && Ok > 0 && (Skip > 0 || Fail > 0);
    public bool IsNoneImported => !FileRejected && Ok == 0;
}
