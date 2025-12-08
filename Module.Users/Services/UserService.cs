using Core.Base;
using Core.Interfaces;
using Core.Models;
using LinqKit;
using Module.Users.Entities;
using Module.Users.Models;
using Module.Users.Models.Requests;

namespace Module.Users.Services
{
    public interface IUserService
    {
        Task<Paginated<UserListItem>> GetAllUsers(SearchUserRequest request);
    }

    public class UserService : BaseService, IUserService
    {
        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Paginated<UserListItem>> GetAllUsers(SearchUserRequest request)
        {
            // Tạo filter
            var filter = PredicateBuilder.New<User>(true);
            if (!string.IsNullOrEmpty(request.Email))
            {
                filter.And(u => u.Email == request.Email);
            }

            var users = await _unitOfWork.Repository<User>().GetAllAsync(
                predicate: filter,
                pageIndex: request.PageIndex,
                pageSize: request.PageSize,
                orderBy: request.)
        }
    }
}
