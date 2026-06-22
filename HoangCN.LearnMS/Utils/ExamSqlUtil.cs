using Dapper;
using System;

namespace HoangCN.LearnMS.Utils
{
    /// <summary>
    /// Tiện ích quản lý tập trung các câu lệnh Raw SQL phục vụ truy vấn Dapper cho module Exam
    /// </summary>
    public static class ExamSqlUtil
    {
        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách mối liên kết đề thi - câu hỏi theo ExamId
        /// </summary>
        public static string BuildQueryExamQuestionsByExamId(Guid examId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("ExamId", examId);
            return "SELECT * FROM ExamQuestion WHERE ExamId = @ExamId";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy số lượng câu hỏi của tất cả đề thi
        /// </summary>
        public static string BuildQueryQuestionCounts(out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            return "SELECT ExamId, COUNT(*) as Count FROM ExamQuestion GROUP BY ExamId";
        }
    }
}
