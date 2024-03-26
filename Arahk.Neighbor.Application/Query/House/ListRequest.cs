using Arahk.Neighbor.Domain.Administrator;
using MediatR;

namespace Arahk.Neighbor.Application.Query.House
{
    public class ListRequest : IRequest<IEnumerable<HouseEntity>>
    {
    }
}
