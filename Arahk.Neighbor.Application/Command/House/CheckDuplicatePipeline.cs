using Arahk.Neighbor.Application.Utils;
using Arahk.Neighbor.Domain.Administrator;
using MediatR;

namespace Arahk.Neighbor.Application.Commmand.House;

public class CheckDuplicatePipeline : IPipelineBehavior<CreateRequest, HouseEntity>
{
    private readonly HouseDuplicateChecker houseDuplicateChecker;

    public CheckDuplicatePipeline(HouseDuplicateChecker houseDuplicateChecker)
    {
        this.houseDuplicateChecker = houseDuplicateChecker;
    }

    public async Task<HouseEntity> Handle(CreateRequest request, RequestHandlerDelegate<HouseEntity> next, CancellationToken cancellationToken)
    {
        bool isExisted = await houseDuplicateChecker.IsDuplicate(request.AddressName);

        if (isExisted)
        {
            throw new Exception("House already exists");
        }

        return await next();
    }
}