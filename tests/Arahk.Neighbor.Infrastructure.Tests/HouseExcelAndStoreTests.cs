using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Infrastructure.Excel;
using Arahk.Neighbor.Infrastructure.Persistence;
using ClosedXML.Excel;
using FluentAssertions;

namespace Arahk.Neighbor.Infrastructure.Tests;

public class HouseExcelAndStoreTests
{
    private readonly Guid _community = HouseConstants.DefaultCommunityId;

    [Fact]
    public async Task InMemory_store_crud_and_unique_lookup()
    {
        var store = new InMemoryHouseRepository();
        var now = DateTimeOffset.UtcNow;
        var a = House.Create(_community, "12/1", "ซอย", now);
        var b = House.Create(_community, "12/2", null, now);
        await store.AddAsync(a);
        await store.AddAsync(b);

        (await store.ListByCommunityAsync(_community)).Should().HaveCount(2);
        (await store.FindByHouseNoAsync(_community, " 12/1 "))!.Id.Should().Be(a.Id);

        a.Update("12/1", "ใหม่", now);
        await store.UpdateAsync(a);
        (await store.GetByIdAsync(_community, a.Id))!.Soi.Should().Be("ใหม่");

        await store.DeleteAsync(a);
        (await store.ListByCommunityAsync(_community)).Should().ContainSingle(h => h.Id == b.Id);
    }

    [Fact]
    public void Parser_reads_Houses_sheet_and_headers()
    {
        using var stream = BuildWorkbook("Houses", ("HouseNo", "Soi"), ("12/1", "ซอย"), ("12/2", ""));
        var parser = new ClosedXmlHouseExcelParser();
        var result = parser.Parse(stream, "houses.xlsx", stream.Length);
        result.Succeeded.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].HouseNo.Should().Be("12/1");
        result.Data[1].Soi.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Parser_rejects_wrong_extension_and_sheet()
    {
        var parser = new ClosedXmlHouseExcelParser();
        using var stream = BuildWorkbook("Wrong", ("HouseNo", "Soi"), ("1", null));
        parser.Parse(stream, "houses.csv", stream.Length).ErrorKey.Should().Be(ErrorKeys.HouseImportBadType);
        stream.Position = 0;
        parser.Parse(stream, "houses.xlsx", stream.Length).ErrorKey.Should().Be(ErrorKeys.HouseImportNoSheet);
    }

    [Fact]
    public void Parser_rejects_bad_headers()
    {
        using var stream = BuildWorkbook("Houses", ("No", "Lane"), ("1", "x"));
        var parser = new ClosedXmlHouseExcelParser();
        parser.Parse(stream, "houses.xlsx", stream.Length).ErrorKey.Should().Be(ErrorKeys.HouseImportBadHeaders);
    }

    private static MemoryStream BuildWorkbook(
        string sheetName,
        (string A, string B) headers,
        params (string? A, string? B)[] rows)
    {
        var ms = new MemoryStream();
        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add(sheetName);
            ws.Cell(1, 1).Value = headers.A;
            ws.Cell(1, 2).Value = headers.B;
            for (var i = 0; i < rows.Length; i++)
            {
                if (rows[i].A is not null) ws.Cell(i + 2, 1).Value = rows[i].A;
                if (rows[i].B is not null) ws.Cell(i + 2, 2).Value = rows[i].B;
            }
            wb.SaveAs(ms);
        }
        ms.Position = 0;
        return ms;
    }
}
