using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using System;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Interfaces
{
    public interface IQuizService : IBaseBL<Quiz>
    {
        Task<ResultDto<Quiz>> GetQuizzesPagingAsync(GetRequest request, Guid? currentUserId);
    }
}
