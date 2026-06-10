using HoangCN.LearnMS.Entities;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Exceptions;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý ngân hàng đề trắc nghiệm và câu hỏi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : BaseController<Question>
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService) : base(questionService)
        {
            _questionService = questionService;
        }

        private Guid CheckAuth()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedException("Vui lòng đăng nhập để thực hiện chức năng này");
            }
            return userId;
        }

        /// <summary>
        /// Endpoint API nhận chuỗi JSON trực tiếp từ body để import ngân hàng câu hỏi hàng loạt
        /// Đường dẫn: POST api/questions/bulk/json
        /// </summary>
        [HttpPost("bulk/json")]
        public async Task<IActionResult> ImportBulkFromJsonBody([FromBody] JsonElement jsonBody)
        {
            var userId = CheckAuth();
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

            var userId = CheckAuth();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var jsonContent = await reader.ReadToEndAsync();
                var count = await _questionService.ImportBulkFromJsonAsync(jsonContent, userId);
                return Ok(ApiResponseDto.Success(new { ImportedCount = count }));
            }
        }
    }
}
