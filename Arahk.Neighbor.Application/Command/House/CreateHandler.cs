using Arahk.Neighbor.Application.Data;
using Arahk.Neighbor.Domain.Administrator;
using MediatR;

namespace Arahk.Neighbor.Application.Commmand.House;

public class CreateHandler : IRequestHandler<CreateRequest, HouseEntity>
{
    private readonly IUnitOfWork unitOfWork;

    public CreateHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<HouseEntity> Handle(CreateRequest request, CancellationToken cancellationToken)
    {
        HouseEntity house = new()
        {
            AddressName = request.AddressName
        };

        await unitOfWork.HouseRepository.AddAsync(house);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return house;
    }
}
