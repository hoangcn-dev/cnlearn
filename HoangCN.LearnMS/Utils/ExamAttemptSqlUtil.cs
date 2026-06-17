using Dapper;
using System;

namespace HoangCN.LearnMS.Utils
{
    /// <summary>
    /// Tiện ích quản lý tập trung các câu lệnh Raw SQL phục vụ truy vấn Dapper cho module ExamAttempt
    /// </summary>
    public static class ExamAttemptSqlUtil
    {
        /// <summary>
        /// Tạo câu lệnh SQL đếm số lượt thi/làm bài của người dùng
        /// </summary>
        public static string BuildQueryExamHistoryCount(Guid userId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            return @"
                SELECT COUNT(1) 
                FROM ExamAttempt 
                WHERE UserId = @UserId AND AttemptType IN ('exam', 'quiz', 'practice')";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách phân trang lịch sử thi/làm bài của người dùng
        /// </summary>
        public static string BuildQueryExamHistoryPaging(Guid userId, int pageSize, int offset, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("PageSize", pageSize);
            parameters.Add("Offset", offset);
            return @"
                SELECT 
                    ea.ExamAttemptId,
                    COALESCE(q.Title, e.Name, 'Đề thi không xác định') AS Title,
                    ea.AttemptType,
                    COALESCE(ea.QuizId, ea.ExamId) AS RelatedId,
                    ea.FinishedDate,
                    ea.CorrectCount,
                    ea.TotalQuestions,
                    ea.Score,
                    ea.Duration
                FROM ExamAttempt ea
                LEFT JOIN Exam e ON ea.ExamId = e.ExamId
                LEFT JOIN Quiz q ON ea.QuizId = q.QuizId
                WHERE ea.UserId = @UserId AND AttemptType IN ('exam', 'quiz', 'practice')
                ORDER BY ea.FinishedDate DESC
                LIMIT @PageSize OFFSET @Offset";
        }

        /// <summary>
        /// Tạo câu lệnh SQL đếm số lượt luyện câu hỏi lẻ của người dùng
        /// </summary>
        public static string BuildQueryQuestionHistoryCount(Guid userId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            return @"
                SELECT COUNT(1) 
                FROM ExamAttempt 
                WHERE UserId = @UserId AND AttemptType = 'question'";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách phân trang lịch sử luyện câu hỏi lẻ của người dùng
        /// </summary>
        public static string BuildQueryQuestionHistoryPaging(Guid userId, int pageSize, int offset, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("PageSize", pageSize);
            parameters.Add("Offset", offset);
            return @"
                SELECT 
                    ea.ExamAttemptId,
                    COALESCE(q.StringContent, 'Câu hỏi không xác định') AS Title,
                    ea.AttemptType,
                    ea.QuestionId AS RelatedId,
                    ea.FinishedDate,
                    ea.CorrectCount,
                    ea.TotalQuestions,
                    ea.Score,
                    ea.Duration
                FROM ExamAttempt ea
                LEFT JOIN Question q ON ea.QuestionId = q.QuestionId
                WHERE ea.UserId = @UserId AND AttemptType = 'question'
                ORDER BY ea.FinishedDate DESC
                LIMIT @PageSize OFFSET @Offset";
        }
    }
}
