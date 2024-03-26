using Arahk.Neighbor.Domain.Administrator;
using MediatR;

namespace Arahk.Neighbor.Application.Commmand.House;

public class CreateRequest : IRequest<HouseEntity>
{
    public string AddressName { get; set; } = string.Empty;
}
