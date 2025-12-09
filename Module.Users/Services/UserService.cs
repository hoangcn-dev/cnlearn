using Core.Base;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.Utilities;
using LinqKit;
using Module.Users.Entities;
using Module.Users.Models;
using Module.Users.Models.Requests;

namespace Module.Users.Services
{
    public interface IUserService
    {
        Task<Paginated<UserListItem>> GetAllUsers(SearchUserRequest request);
        Task<UserDetail> GetUserDetail(Guid userId);
        Task<Paginated<UserLogListItem>> GetUserLogs(SearchUserLogRequest request);
        Task<Guid> UpdateUser(Guid userId, UpdateUserRequest request);
        Task<Guid> DeleteUser(Guid userId);
    }

    public class UserService : BaseService, IUserService
    {
        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Paginated<UserListItem>> GetAllUsers(SearchUserRequest request)
        {
            // Build filter
            var filter = PredicateBuilder.New<User>(true);
            if (!string.IsNullOrEmpty(request.Email))
            {
                filter.And(u => u.Email == request.Email);
            }

            var users = await _unitOfWork.Repository<User>().GetPagingAsync(
                predicate: filter,
                pageIndex: request.PageIndex,
                pageSize: request.PageSize,
                orderBy: u => u.FirstName,
                asc: true,
                selector: u => UserListItem.ConvertFromUserToItem(u));

            return users;
        }

        public async Task<UserDetail> GetUserDetail(Guid userId)
        {
            var user = await _unitOfWork.Repository<User>().GetFirstAsync(
                predicate: u => u.Id == userId,
                selector: u => UserDetail.ConvertFromUserToDetail(u)
            );

            if (user is null)
            {
                throw new NotFoundException(StringUtil.ApiMessages.UserNotFound);
            }

            return user;
        }

        public async Task<Paginated<UserLogListItem>> GetUserLogs(SearchUserLogRequest request)
        {
            // Build filter
            var filter = PredicateBuilder.New<UserLog>(true);
            if (request.UserId is not null)
            {
                filter.And(ul => ul.UserId == request.UserId);
            }

            var logs = await _unitOfWork.Repository<UserLog>().GetPagingAsync(
                predicate: filter,
                selector: ul => UserLogListItem.ConvertFromUserLogToItem(ul),
                pageIndex: request.PageIndex,
                pageSize: request.PageSize,
                orderBy: ul => ul.Timestamp,
                asc: false
            );

            return logs;
        }

        public async Task<Guid> UpdateUser(Guid userId, UpdateUserRequest request)
        {
            var user = await _unitOfWork.Repository<User>().GetFirstAsync(
                predicate: u => u.Id == userId
            );

            if (user is null)
            {
                throw new NotFoundException(StringUtil.ApiMessages.UserNotFound);
            }

            // Handle lock / unlock account
            if (user.IsActived != request.IsActived)
            {
                user.IsActived = request.IsActived;
                if (user.IsActived)
                {
                    user.AddLog(StringUtil.LogMessages.UserIsUnlocked, isSystemAction: true);
                }
                else
                {
                    if (string.IsNullOrEmpty(request.LockReason))
                    {
                        throw new BadRequestException(StringUtil.ApiMessages.LockReasonNotSet);
                    }

                    user.AddLog(StringUtil.LogMessages.UserIsLocked(request.LockReason), isSystemAction: true);
                }
            }

            user.Note = request.Note;

            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return userId;
        }

        public async Task<Guid> DeleteUser(Guid userId)
        {
            var user = await _unitOfWork.Repository<User>().GetFirstAsync(
                predicate: u => u.Id == userId
            );

            if (user is null)
            {
                throw new NotFoundException(StringUtil.ApiMessages.UserNotFound);
            }

            await _unitOfWork.Repository<User>().DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return userId;
        }

    }
}
