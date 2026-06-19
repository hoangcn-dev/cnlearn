using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Base;
using HoangCN.LearnMS.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý danh mục câu hỏi trắc nghiệm
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : CRUDController<QuestionCategory>
    {
        public CategoriesController(IBaseBL<QuestionCategory> questionCategoryService) : base(questionCategoryService)
        {
        }

        protected override void ConfigurePolicies(AuthActionPolicyBuilder builder)
        {
            base.ConfigurePolicies(builder);
        }
    }
}
