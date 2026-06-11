using HoangCN.LearnMS.Entities;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Enums;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Lấy danh sách câu hỏi phân trang kèm chi tiết đáp án và danh mục
        /// Đường dẫn: POST api/questions/paging-details
        /// </summary>
        [HttpPost("paging-details")]
        public async Task<IActionResult> GetPagingDetails([FromBody] GetRequest request)
        {
            var userId = CheckAuth();
            var res = await _questionService.GetQuestionDetailsPagingAsync(request, userId);
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lấy danh sách câu hỏi đã lưu phân trang của người dùng hiện tại
        /// Đường dẫn: POST api/questions/saved/paging
        /// </summary>
        [HttpPost("saved/paging")]
        public async Task<IActionResult> GetSavedPagingDetails([FromBody] GetRequest request)
        {
            var userId = CheckAuth();
            request ??= new GetRequest();
            request.Filters ??= new List<Filter>();
            request.Filters.Add(new Filter
            {
                Property = "IsSaved",
                Operator = FilterOperator.Equal,
                Value = "true",
                Type = FilterType.String
            });
            var res = await _questionService.GetQuestionDetailsPagingAsync(request, userId);
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lấy danh sách câu hỏi đã làm phân trang của người dùng hiện tại
        /// Đường dẫn: POST api/questions/done/paging
        /// </summary>
        [HttpPost("done/paging")]
        public async Task<IActionResult> GetDonePagingDetails([FromBody] GetRequest request)
        {
            var userId = CheckAuth();
            request ??= new GetRequest();
            request.Filters ??= new List<Filter>();
            request.Filters.Add(new Filter
            {
                Property = "IsDone",
                Operator = FilterOperator.Equal,
                Value = "true",
                Type = FilterType.String
            });
            var res = await _questionService.GetQuestionDetailsPagingAsync(request, userId);
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lấy chi tiết câu hỏi theo ID kèm đáp án và danh mục
        /// Đường dẫn: GET api/questions/details/{id}
        /// </summary>
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetDetailsById(Guid id)
        {
            var userId = CheckAuth();
            var res = await _questionService.GetQuestionDetailsByIdAsync(id, userId);
            if (res == null)
            {
                throw new NotFoundException("Câu hỏi không tồn tại");
            }
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lưu danh sách câu hỏi kèm đáp án và danh mục (Thêm mới/Cập nhật)
        /// Đường dẫn: POST api/questions/save-details
        /// </summary>
        [HttpPost("save-details")]
        public async Task<IActionResult> SaveDetails([FromBody] List<QuestionDetailsDto> questionsDto)
        {
            var userId = CheckAuth();
            await _questionService.SaveQuestionDetailsAsync(questionsDto, userId);
            return Ok(ApiResponseDto.Success());
        }
    }
}
