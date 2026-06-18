using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý ngân hàng đề trắc nghiệm và câu hỏi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : CRUDController<Question>
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

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
            {
                return parsedId;
            }
            return null;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var request = GetRequest.GetAllRequest();
            var res = await _questionService.GetQuestionDetailsPagingAsync(request, GetCurrentUserId() ?? Guid.Empty);
            return Ok(ApiResponseDto.Success(res.Items)); // Trả về Items để khớp cấu trúc mảng của GetAll
        }

        [HttpPost("paging")]
        public override async Task<IActionResult> GetPaging([FromBody] GetRequest request)
        {
            var res = await _questionService.GetQuestionDetailsPagingAsync(request, GetCurrentUserId() ?? Guid.Empty);
            return Ok(ApiResponseDto.Success(res));
        }


        /// <summary>
        /// Lấy danh sách câu hỏi phân trang kèm chi tiết đáp án và danh mục
        /// Đường dẫn: POST api/questions/paging-details
        /// </summary>
        [HttpPost("paging-details")]
        public async Task<IActionResult> GetPagingDetails([FromBody] GetRequest request)
        {
            var userId = GetCurrentUserId() ?? Guid.Empty;
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
        public async Task<IActionResult> SaveDetails([FromBody] List<QuestionDetailDto> questionsDto)
        {
            var userId = CheckAuth();
            await _questionService.SaveQuestionDetailsAsync(questionsDto, userId);
            return Ok(ApiResponseDto.Success());
        }

        /// <summary>
        /// API kiểm tra đáp án của một câu hỏi (Dành cho Luyện tập/Thi)
        /// </summary>
        [HttpPost("check-answer")]
        public async Task<IActionResult> CheckAnswer([FromBody] QuestionCheckDto dto)
        {
            var result = await _questionService.CheckAnswerAsync(dto);
            return Ok(ApiResponseDto.Success(result));
        }
    }
}
