
using Arahk.Neighbor.Application.Data;
using Arahk.Neighbor.Domain.Administrative;
using MediatR;

namespace Arahk.Neighbor.Application.Query.House
{
    public class ListHandler : IRequestHandler<ListRequest, IEnumerable<HouseEntity>>
    {
        private readonly IUnitOfWork unitOfWork;

        public ListHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<HouseEntity>> Handle(ListRequest request, CancellationToken cancellationToken)
        {
            return unitOfWork.HouseRepository.GetAllAsync(cancellationToken);
        }
    }
}
