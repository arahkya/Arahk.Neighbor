using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Domain.Constants;
using ClosedXML.Excel;

namespace Arahk.Neighbor.Infrastructure.Excel;

public class ClosedXmlHouseExcelParser : IHouseExcelParser
{
    public Result<IReadOnlyList<HouseImportRow>> Parse(Stream stream, string fileName, long contentLength)
    {
        if (string.IsNullOrWhiteSpace(fileName) ||
            !fileName.EndsWith(HouseConstants.ExcelExtension, StringComparison.OrdinalIgnoreCase))
            return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportBadType);

        if (contentLength > HouseConstants.ExcelMaxBytes)
            return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportTooLarge);

        try
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets
                .FirstOrDefault(ws => string.Equals(ws.Name, HouseConstants.ExcelSheetName, StringComparison.Ordinal));

            if (worksheet is null)
                return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportNoSheet);

            var used = worksheet.RangeUsed();
            if (used is null)
                return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportEmptyFile);

            var firstRow = used.FirstRow();
            var headerMap = ReadHeaders(firstRow);
            if (headerMap is null)
                return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportBadHeaders);

            if (!headerMap.ContainsKey(HouseConstants.ExcelColumnHouseNo) ||
                !headerMap.ContainsKey(HouseConstants.ExcelColumnSoi))
                return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportMissingColumns);

            var houseNoCol = headerMap[HouseConstants.ExcelColumnHouseNo];
            var soiCol = headerMap[HouseConstants.ExcelColumnSoi];

            var rows = new List<HouseImportRow>();
            var lastRowNumber = used.LastRow().RowNumber();

            for (var r = firstRow.RowNumber() + 1; r <= lastRowNumber; r++)
            {
                var houseNo = CellText(worksheet.Cell(r, houseNoCol));
                var soi = CellText(worksheet.Cell(r, soiCol));

                if (string.IsNullOrWhiteSpace(houseNo) && string.IsNullOrWhiteSpace(soi))
                    continue;

                rows.Add(new HouseImportRow(r, houseNo, soi));
            }

            if (rows.Count == 0)
                return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportEmptyFile);

            if (rows.Count > HouseConstants.ExcelMaxDataRows)
                return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportTooManyRows);

            return Result<IReadOnlyList<HouseImportRow>>.Ok(rows);
        }
        catch (Exception)
        {
            return Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportGeneric);
        }
    }

    private static Dictionary<string, int>? ReadHeaders(IXLRangeRow firstRow)
    {
        var map = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var cell in firstRow.CellsUsed())
        {
            var name = cell.GetString()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
                continue;
            map[name] = cell.Address.ColumnNumber;
        }

        // Prefer exact A=HouseNo B=Soi when both present; still accept by name
        if (!map.ContainsKey(HouseConstants.ExcelColumnHouseNo) ||
            !map.ContainsKey(HouseConstants.ExcelColumnSoi))
            return null;

        return map;
    }

    private static string? CellText(IXLCell cell)
    {
        if (cell.IsEmpty())
            return null;
        return cell.GetFormattedString();
    }
}
