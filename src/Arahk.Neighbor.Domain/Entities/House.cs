namespace Arahk.Neighbor.Domain.Entities;

public class House
{
    public Guid Id { get; private set; }
    public Guid CommunityId { get; private set; }
    public string HouseNo { get; private set; } = string.Empty;
    public string? Soi { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private House() { }

    public static House Create(
        Guid communityId,
        string houseNo,
        string? soi,
        DateTimeOffset now)
    {
        var normalizedNo = NormalizeHouseNo(houseNo);
        var normalizedSoi = NormalizeSoi(soi);
        if (string.IsNullOrEmpty(normalizedNo))
            throw new ArgumentException("HouseNo is required.", nameof(houseNo));

        return new House
        {
            Id = Guid.NewGuid(),
            CommunityId = communityId,
            HouseNo = normalizedNo,
            Soi = normalizedSoi,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Update(string houseNo, string? soi, DateTimeOffset now)
    {
        var normalizedNo = NormalizeHouseNo(houseNo);
        if (string.IsNullOrEmpty(normalizedNo))
            throw new ArgumentException("HouseNo is required.", nameof(houseNo));

        HouseNo = normalizedNo;
        Soi = NormalizeSoi(soi);
        UpdatedAt = now;
    }

    public static string NormalizeHouseNo(string? houseNo) =>
        houseNo?.Trim() ?? string.Empty;

    public static string? NormalizeSoi(string? soi)
    {
        if (string.IsNullOrWhiteSpace(soi))
            return null;
        var trimmed = soi.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }
}
