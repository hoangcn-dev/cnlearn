using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.LearnMS.Entities;

namespace HoangCN.LearnMS.Interfaces
{
    public interface IQuizService : IBaseBL<Quiz>
    {
        Task<ResultDto<Quiz>> GetQuizzesPagingAsync(GetRequest request, Guid? currentUserId);
    }
}
