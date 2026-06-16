using HoangCN.Core.BL.Interfaces;
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
    //[Authorize(Roles = "Admin")] // Giới hạn quyền Admin
    public class CategoriesController : BaseController<QuestionCategory>
    {
        public CategoriesController(IBaseBL<QuestionCategory> questionCategoryService) : base(questionCategoryService)
        {
        }
    }
}
