using Core.Interfaces;

namespace Core.Base
{
    public class BaseService : IBaseService
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
