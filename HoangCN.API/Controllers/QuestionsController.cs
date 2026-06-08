using HoangCN.Common.Model.Entities;
using HoangCN.Common.Model.DTOs;
using HoangCN.Common.Exceptions;
using HoangCN.LearnMS.Interfaces;
using HoangCN.UserManagement.Interfaces;
using HoangCN.UserManagement.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace HoangCN.API.Controllers
{
    /// <summary>
    /// API Quản lý ngân hàng đề trắc nghiệm và câu hỏi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = nameof(RoleNames.Admin))] // Giới hạn quyền Admin
    public class QuestionsController : BaseController<Question>
    {
        private readonly IQuestionService _questionService;
        private readonly IUserService _userService;

        public QuestionsController(IQuestionService questionService, IUserService userService) : base(questionService)
        {
            _questionService = questionService;
            _userService = userService;
        }

        /// <summary>
        /// Endpoint API nhận chuỗi JSON trực tiếp từ body để import ngân hàng câu hỏi hàng loạt
        /// Đường dẫn: POST api/questions/bulk/json
        /// </summary>
        [HttpPost("bulk/json")]
        public async Task<IActionResult> ImportBulkFromJsonBody([FromBody] JsonElement jsonBody)
        {
            var userId = await _userService.CheckAuth(User);
            var jsonContent = jsonBody.GetRawText();
            var count = await _questionService.ImportBulkFromJsonAsync(jsonContent, userId);
            return Ok(ApiResponseDto.Success(new { ImportedCount = count }));
        }

        /// <summary>
        /// Endpoint API nhận file .json upload lên để import ngân hàng câu hỏi hàng loạt
        /// Đường dẫn: POST api/questions/bulk/json-file
        /// </summary>
        [HttpPost("bulk/json-file")]
        public async Task<IActionResult> ImportBulkFromFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException("File tải lên không hợp lệ hoặc bị rỗng.");
            }

            var userId = await _userService.CheckAuth(User);
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var jsonContent = await reader.ReadToEndAsync();
                var count = await _questionService.ImportBulkFromJsonAsync(jsonContent, userId);
                return Ok(ApiResponseDto.Success(new { ImportedCount = count }));
            }
        }
    }
}
