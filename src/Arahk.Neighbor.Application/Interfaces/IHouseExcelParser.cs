using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Models;

namespace Arahk.Neighbor.Application.Interfaces;

public interface IHouseExcelParser
{
    /// <summary>Parse .xlsx stream; returns file-level failure or list of data rows (blank rows filtered).</summary>
    Result<IReadOnlyList<HouseImportRow>> Parse(Stream stream, string fileName, long contentLength);
}
