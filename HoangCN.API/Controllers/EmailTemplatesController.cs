using HoangCN.Common.Model.DTOs;
using HoangCN.MainSystem.Interfaces;
using HoangCN.Common.Model.Entities;
using HoangCN.UserManagement.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HoangCN.API.Controllers
{
    /// <summary>
    /// API Quản lý các mẫu Template Email - Chỉ dành riêng cho Admin
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(RoleNames.Admin))]
    public class EmailTemplatesController : ControllerBase
    {
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailTemplatesController(IEmailTemplateService emailTemplateService)
        {
            _emailTemplateService = emailTemplateService;
        }

        /// <summary>
        /// Lấy tất cả danh sách các mẫu template email hiện có
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var templates = await _emailTemplateService.GetAllTemplatesAsync();
            return Ok(ApiResponseDto.Success(templates));
        }

        /// <summary>
        /// Lấy chi tiết một mẫu template theo mã code
        /// </summary>
        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var template = await _emailTemplateService.GetTemplateAsync(code);
            return Ok(ApiResponseDto.Success(template));
        }

        /// <summary>
        /// Thêm mới hoặc cập nhật thông tin mẫu template email
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] EmailTemplate template)
        {
            await _emailTemplateService.SaveTemplateAsync(template);
            return Ok(ApiResponseDto.Success("Lưu mẫu email thành công."));
        }
    }
}
