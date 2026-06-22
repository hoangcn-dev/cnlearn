using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;

namespace HoangCN.LearnMS.Interfaces
{
    /// <summary>
    /// Giao diện nghiệp vụ đề thi
    /// </summary>
    public interface IExamService : IBaseBL<Exam>
    {
        ///// <summary>
        ///// Lưu chi tiết đề thi và danh sách câu hỏi đi kèm trong một Transaction
        ///// </summary>
        //Task<Guid> SaveExamDetailsAsync(ExamSaveDto dto, Guid currentUserId);

        ///// <summary>
        ///// Lấy danh sách đề thi kèm lọc phân quyền
        ///// </summary>
        //Task<ResultDto<Exam>> GetExamsPagingAsync(GetRequest request, Guid? currentUserId);

        ///// <summary>
        ///// Lấy số lượng câu hỏi của tất cả đề thi
        ///// </summary>
        //Task<Dictionary<Guid, int>> GetQuestionCountsAsync();
    }
}
