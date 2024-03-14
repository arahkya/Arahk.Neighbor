using Arahk.Neighbor.Domain.Administrative;
using MediatR;

namespace Arahk.Neighbor.Application.Query.House
{
    public class ListRequest : IRequest<IEnumerable<HouseEntity>>
    {
    }
}
