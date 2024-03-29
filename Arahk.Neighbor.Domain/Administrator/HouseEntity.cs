using Arahk.Neighbor.Domain.Common;

namespace Arahk.Neighbor.Domain.Administrator;

public class HouseEntity : IEntity
{
    public Guid Id { get; internal set; }

    public string AddressName { get; internal set; } = string.Empty;
}

