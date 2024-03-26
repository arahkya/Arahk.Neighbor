using Arahk.Neighbor.Application.Data;
using Arahk.Neighbor.Domain.Administrator;

namespace Arahk.Neighbor.Application.Utils;

public class HouseDuplicateChecker
{
    private readonly IUnitOfWork unitOfWork;

    public HouseDuplicateChecker(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<bool> IsDuplicate(string addressName)
    {
        HouseEntity? houseEntity = await unitOfWork.HouseRepository.GetByConditionAsync(x => x.AddressName == addressName);

        if (houseEntity != null)
        {
            return true;
        }

        return false;
    }
}
