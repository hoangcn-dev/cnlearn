using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Base;
using HoangCN.LearnMS.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.BL.Attributes.AuthAction;

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
            builder.Disable(nameof(GetById));
            builder.Disable(nameof(GetPaging));
            builder.Protect(nameof(Insert), nameof(RoleNames.Admin));
            builder.Protect(nameof(Update), nameof(RoleNames.Admin));
            builder.Protect(nameof(Delete), nameof(RoleNames.Admin));
        }
    }
}
