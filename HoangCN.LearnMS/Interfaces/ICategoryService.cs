using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.Entities;

namespace HoangCN.LearnMS.Interfaces
{
    public interface ICategoryService : IBaseBL<QuestionCategory>
    {
        Task<bool> ValidateLeafCategories(List<Guid> categoryIds);
    }
}
