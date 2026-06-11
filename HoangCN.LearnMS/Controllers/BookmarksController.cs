using HoangCN.LearnMS.Entities;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý đánh dấu / lưu câu hỏi và đề thi của người dùng
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookmarksController : ControllerBase
    {
        private readonly IBaseBL<UserSavedQuestion> _savedQuestionBL;
        private readonly IBaseBL<UserSavedExam> _savedExamBL;

        public BookmarksController(
            IBaseBL<UserSavedQuestion> savedQuestionBL,
            IBaseBL<UserSavedExam> savedExamBL)
        {
            _savedQuestionBL = savedQuestionBL;
            _savedExamBL = savedExamBL;
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
        /// Bật/Tắt trạng thái lưu câu hỏi của người dùng
        /// Đường dẫn: POST api/bookmarks/question/toggle/{id}
        /// </summary>
        [HttpPost("question/toggle/{id}")]
        public async Task<IActionResult> ToggleQuestion(Guid id)
        {
            var userId = CheckAuth();
            
            var existing = await _savedQuestionBL.Get<UserSavedQuestion>(new GetRequest
            {
                Filters = new List<Filter>
                {
                    new Filter { Property = "UserId", Operator = FilterOperator.Equal, Value = userId, Type = FilterType.String },
                    new Filter { Property = "QuestionId", Operator = FilterOperator.Equal, Value = id, Type = FilterType.String }
                },
                FilterGroupType = FilterGroupType.And
            });

            if (existing.Items.Count > 0)
            {
                await _savedQuestionBL.Delete(new DeleteRequest
                {
                    Ids = new List<Guid> { existing.Items[0].UserSavedQuestionId }
                });
                return Ok(ApiResponseDto.Success(false)); // Đã bỏ lưu
            }
            else
            {
                var newBookmark = new UserSavedQuestion
                {
                    UserSavedQuestionId = Guid.NewGuid(),
                    UserId = userId,
                    QuestionId = id,
                    CreatedBy = userId.ToString(),
                    CreatedDate = DateTime.Now,
                    State = HoangCN.Core.Common.Enums.ModelState.Insert
                };
                await _savedQuestionBL.Save(new List<UserSavedQuestion> { newBookmark });
                return Ok(ApiResponseDto.Success(true)); // Đã lưu
            }
        }

        /// <summary>
        /// Bật/Tắt trạng thái lưu đề thi của người dùng
        /// Đường dẫn: POST api/bookmarks/exam/toggle/{id}
        /// </summary>
        [HttpPost("exam/toggle/{id}")]
        public async Task<IActionResult> ToggleExam(Guid id)
        {
            var userId = CheckAuth();
            
            var existing = await _savedExamBL.Get<UserSavedExam>(new GetRequest
            {
                Filters = new List<Filter>
                {
                    new Filter { Property = "UserId", Operator = FilterOperator.Equal, Value = userId, Type = FilterType.String },
                    new Filter { Property = "ExamId", Operator = FilterOperator.Equal, Value = id, Type = FilterType.String }
                },
                FilterGroupType = FilterGroupType.And
            });

            if (existing.Items.Count > 0)
            {
                await _savedExamBL.Delete(new DeleteRequest
                {
                    Ids = new List<Guid> { existing.Items[0].UserSavedExamId }
                });
                return Ok(ApiResponseDto.Success(false)); // Đã bỏ lưu
            }
            else
            {
                var newBookmark = new UserSavedExam
                {
                    UserSavedExamId = Guid.NewGuid(),
                    UserId = userId,
                    ExamId = id,
                    CreatedBy = userId.ToString(),
                    CreatedDate = DateTime.Now,
                    State = HoangCN.Core.Common.Enums.ModelState.Insert
                };
                await _savedExamBL.Save(new List<UserSavedExam> { newBookmark });
                return Ok(ApiResponseDto.Success(true)); // Đã lưu
            }
        }

        /// <summary>
        /// Lấy toàn bộ danh sách ID câu hỏi đã lưu của người dùng hiện tại
        /// Đường dẫn: GET api/bookmarks/questions/ids
        /// </summary>
        [HttpGet("questions/ids")]
        public async Task<IActionResult> GetSavedQuestionIds()
        {
            var userId = CheckAuth();
            var bookmarks = await _savedQuestionBL.Get<UserSavedQuestion>(new GetRequest
            {
                Filters = new List<Filter>
                {
                    new Filter { Property = "UserId", Operator = FilterOperator.Equal, Value = userId, Type = FilterType.String }
                }
            });
            var ids = new List<Guid>();
            foreach (var item in bookmarks.Items)
            {
                ids.Add(item.QuestionId);
            }
            return Ok(ApiResponseDto.Success(ids));
        }

        /// <summary>
        /// Lấy toàn bộ danh sách ID đề thi đã lưu của người dùng hiện tại
        /// Đường dẫn: GET api/bookmarks/exams/ids
        /// </summary>
        [HttpGet("exams/ids")]
        public async Task<IActionResult> GetSavedExamIds()
        {
            var userId = CheckAuth();
            var bookmarks = await _savedExamBL.Get<UserSavedExam>(new GetRequest
            {
                Filters = new List<Filter>
                {
                    new Filter { Property = "UserId", Operator = FilterOperator.Equal, Value = userId, Type = FilterType.String }
                }
            });
            var ids = new List<Guid>();
            foreach (var item in bookmarks.Items)
            {
                ids.Add(item.ExamId);
            }
            return Ok(ApiResponseDto.Success(ids));
        }
    }
}
