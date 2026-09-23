using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.Services;

namespace Arahk.Neighbor.Application.Services;

public class HouseService
{
    private readonly IHouseRepository _houses;
    private readonly IHouseExcelParser _excel;
    private readonly IClock _clock;

    public HouseService(IHouseRepository houses, IHouseExcelParser excel, IClock clock)
    {
        _houses = houses;
        _excel = excel;
        _clock = clock;
    }

    public async Task<IReadOnlyList<HouseDto>> ListAsync(
        Guid communityId,
        CancellationToken ct = default)
    {
        var list = await _houses.ListByCommunityAsync(communityId, ct);
        return list
            .OrderBy(h => h.HouseNo, StringComparer.Ordinal)
            .Select(ToDto)
            .ToList();
    }

    public async Task<Result<HouseDto>> CreateAsync(
        Guid communityId,
        CreateHouseRequest request,
        CancellationToken ct = default)
    {
        var fieldErrors = ValidateFields(request.HouseNo, request.Soi);
        if (fieldErrors.Count > 0)
            return Result<HouseDto>.FailFields(fieldErrors);

        var houseNo = House.NormalizeHouseNo(request.HouseNo);
        var existing = await _houses.FindByHouseNoAsync(communityId, houseNo, ct);
        if (existing is not null)
            return Result<HouseDto>.FailFields(new Dictionary<string, string>
            {
                ["houseNo"] = ErrorKeys.HouseNoDuplicate
            });

        var house = House.Create(communityId, houseNo, request.Soi, _clock.UtcNow);
        await _houses.AddAsync(house, ct);
        return Result<HouseDto>.Ok(ToDto(house));
    }

    public async Task<Result<HouseDto>> UpdateAsync(
        Guid communityId,
        UpdateHouseRequest request,
        CancellationToken ct = default)
    {
        var fieldErrors = ValidateFields(request.HouseNo, request.Soi);
        if (fieldErrors.Count > 0)
            return Result<HouseDto>.FailFields(fieldErrors);

        var house = await _houses.GetByIdAsync(communityId, request.Id, ct);
        if (house is null)
            return Result<HouseDto>.Fail(ErrorKeys.HouseNotFound);

        var houseNo = House.NormalizeHouseNo(request.HouseNo);
        var duplicate = await _houses.FindByHouseNoAsync(communityId, houseNo, ct);
        if (duplicate is not null && duplicate.Id != house.Id)
            return Result<HouseDto>.FailFields(new Dictionary<string, string>
            {
                ["houseNo"] = ErrorKeys.HouseNoDuplicate
            });

        house.Update(houseNo, request.Soi, _clock.UtcNow);
        await _houses.UpdateAsync(house, ct);
        return Result<HouseDto>.Ok(ToDto(house));
    }

    public async Task<Result> DeleteAsync(
        Guid communityId,
        Guid id,
        CancellationToken ct = default)
    {
        var house = await _houses.GetByIdAsync(communityId, id, ct);
        if (house is null)
            return Result.Fail(ErrorKeys.HouseNotFound);

        await _houses.DeleteAsync(house, ct);
        return Result.Ok();
    }

    public async Task<Result<HouseImportResult>> ImportAsync(
        Guid communityId,
        Stream stream,
        string fileName,
        long contentLength,
        CancellationToken ct = default)
    {
        var parsed = _excel.Parse(stream, fileName, contentLength);
        if (!parsed.Succeeded)
        {
            return Result<HouseImportResult>.Ok(new HouseImportResult
            {
                FileRejected = true,
                FileErrorKey = parsed.ErrorKey
            });
        }

        var rows = parsed.Data ?? Array.Empty<HouseImportRow>();
        if (rows.Count == 0)
        {
            return Result<HouseImportResult>.Ok(new HouseImportResult
            {
                FileRejected = true,
                FileErrorKey = ErrorKeys.HouseImportEmptyFile
            });
        }

        var existing = await _houses.ListByCommunityAsync(communityId, ct);
        var existingNos = new HashSet<string>(
            existing.Select(h => h.HouseNo),
            StringComparer.Ordinal);
        var seenInFile = new HashSet<string>(StringComparer.Ordinal);

        var ok = 0;
        var skip = 0;
        var fail = 0;
        var issues = new List<HouseImportRowIssue>();
        var now = _clock.UtcNow;

        foreach (var row in rows)
        {
            var houseNo = House.NormalizeHouseNo(row.HouseNo);
            var soi = House.NormalizeSoi(row.Soi);

            if (string.IsNullOrEmpty(houseNo) && soi is null)
                continue; // blank row already filtered by parser, but be safe

            if (string.IsNullOrEmpty(houseNo))
            {
                fail++;
                issues.Add(new HouseImportRowIssue(row.ExcelRowNumber, "houses.import.result.row_missing_no"));
                continue;
            }

            if (houseNo.Length > HouseConstants.HouseNoMaxLength ||
                (soi is not null && soi.Length > HouseConstants.SoiMaxLength))
            {
                fail++;
                issues.Add(new HouseImportRowIssue(row.ExcelRowNumber, "houses.import.result.row_too_long"));
                continue;
            }

            if (existingNos.Contains(houseNo))
            {
                skip++;
                issues.Add(new HouseImportRowIssue(row.ExcelRowNumber, "houses.import.result.row_dup_existing"));
                continue;
            }

            if (!seenInFile.Add(houseNo))
            {
                skip++;
                issues.Add(new HouseImportRowIssue(row.ExcelRowNumber, "houses.import.result.row_dup_file"));
                continue;
            }

            var house = House.Create(communityId, houseNo, soi, now);
            await _houses.AddAsync(house, ct);
            existingNos.Add(houseNo);
            ok++;
        }

        return Result<HouseImportResult>.Ok(new HouseImportResult
        {
            Ok = ok,
            Skip = skip,
            Fail = fail,
            Issues = issues
        });
    }

    private static Dictionary<string, string> ValidateFields(string? houseNo, string? soi)
    {
        var errors = new Dictionary<string, string>();
        var noErr = HouseFieldRules.ValidateHouseNo(houseNo);
        if (noErr is not null) errors["houseNo"] = noErr;
        var soiErr = HouseFieldRules.ValidateSoi(soi);
        if (soiErr is not null) errors["soi"] = soiErr;
        return errors;
    }

    private static HouseDto ToDto(House h) => new(h.Id, h.HouseNo, h.Soi);
}
