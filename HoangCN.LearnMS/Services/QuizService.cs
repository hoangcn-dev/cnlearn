using HoangCN.Core.BL.Base;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace HoangCN.LearnMS.Services
{
    public class QuizService : BaseBL<Quiz>, IQuizService
    {
        public QuizService(IBaseReadDL baseReadDL, IBaseWriteDL baseWriteDL, IHttpContextAccessor httpContextAccessor) 
            : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
        }

        public async Task<ResultDto<Quiz>> GetQuizzesPagingAsync(GetRequest request, Guid? currentUserId)
        {
            Expression<Func<Quiz, bool>>? condition = null;
            if (currentUserId.HasValue)
            {
                condition = q => !q.IsDraft || q.UserId == currentUserId.Value;
            }
            else
            {
                condition = q => !q.IsDraft;
            }
            var result = await base.Get<Quiz>(request, condition);
            
            if (currentUserId.HasValue && result != null && result.Items != null)
            {
                foreach (var quiz in result.Items)
                {
                    quiz.IsMyCreated = quiz.UserId == currentUserId.Value;
                }
            }

            return result!;
        }
    }
}
