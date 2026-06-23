using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;

namespace HoangCN.LearnMS.Services
{
    public class CategoryService : BaseBL<QuestionCategory>, ICategoryService
    {
        public CategoryService(
            IBaseReadDL baseReadDL, 
            IBaseWriteDL baseWriteDL, 
            IHttpContextAccessor httpContextAccessor) : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
        }

        public async Task<bool> ValidateLeafCategories(List<Guid> categoryIds)
        {
            var childCount = await CountByCondition(c => c.ParentId != null && categoryIds.Contains((Guid)c.ParentId));
            return childCount > 0;
        }
    }
}
