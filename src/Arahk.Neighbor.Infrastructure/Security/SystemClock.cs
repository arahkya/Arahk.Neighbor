using Arahk.Neighbor.Application.Interfaces;

namespace Arahk.Neighbor.Infrastructure.Security;

public class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
