namespace Arahk.Neighbor.Application.Interfaces;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
