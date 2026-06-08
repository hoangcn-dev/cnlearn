using HoangCN.BL.Interfaces;
using HoangCN.Common.Model.Entities;
using HoangCN.UserManagement.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoangCN.API.Controllers
{
    /// <summary>
    /// API Quản lý danh mục câu hỏi trắc nghiệm
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(RoleNames.Admin))] // Giới hạn quyền Admin
    public class QuestionCategoriesController : BaseController<QuestionCategory>
    {
        public QuestionCategoriesController(IBaseBL<QuestionCategory> questionCategoryService) : base(questionCategoryService)
        {
        }
    }
}
