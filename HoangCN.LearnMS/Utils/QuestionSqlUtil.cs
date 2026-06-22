using Dapper;
using System;
using System.Collections.Generic;

namespace HoangCN.LearnMS.Utils
{
    /// <summary>
    /// Tiện ích quản lý tập trung các câu lệnh Raw SQL phục vụ truy vấn Dapper cho module Question
    /// </summary>
    public static class QuestionSqlUtil
    {
        /// <summary>
        /// Tạo câu lệnh SQL để lấy danh sách câu trả lời theo danh sách QuestionId
        /// </summary>
        public static string BuildQueryAnswersByQuestionIds(List<Guid> questionIds, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("Ids", questionIds);
            return "SELECT * FROM QuestionAnswer WHERE QuestionId IN @Ids";
        }

        /// <summary>
        /// Tạo câu lệnh SQL để lấy danh sách câu trả lời theo một QuestionId
        /// </summary>
        public static string BuildQueryAnswersByQuestionId(Guid questionId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("Id", questionId);
            return "SELECT * FROM QuestionAnswer WHERE QuestionId = @Id ORDER BY OrderInList";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách ID câu hỏi đã làm của người dùng
        /// </summary>
        public static string BuildQueryDoneQuestionIds(Guid userId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            return @"
                SELECT DISTINCT QuestionId 
                FROM ExamAttempt 
                WHERE UserId = @UserId AND AttemptType = 'question' AND QuestionId IS NOT NULL
                UNION
                SELECT DISTINCT d.QuestionId 
                FROM ExamAttemptDetail d
                INNER JOIN ExamAttempt a ON d.ExamAttemptId = a.ExamAttemptId
                WHERE a.UserId = @UserId AND a.AttemptType != 'question'";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách ID câu hỏi đã lưu của người dùng
        /// </summary>
        public static string BuildQuerySavedQuestionIds(Guid userId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            return "SELECT DISTINCT QuestionId FROM UserSavedQuestion WHERE UserId = @UserId";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách ID Exam được phép xem (Công khai hoặc Của mình)
        /// </summary>
        public static string BuildQueryAllowedExamIds(Guid? currentUserId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            if (currentUserId.HasValue)
            {
                parameters.Add("CurrentUserId", currentUserId.Value);
                return "SELECT ExamId FROM Exam WHERE (AccessType = 1 AND IsDraft = 0 OR UserId = @CurrentUserId)";
            }
            return "SELECT ExamId FROM Exam WHERE AccessType = 1 AND IsDraft = 0";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách ID Quiz được phép xem (Công khai hoặc Của mình)
        /// </summary>
        public static string BuildQueryAllowedQuizIds(Guid? currentUserId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            if (currentUserId.HasValue)
            {
                parameters.Add("CurrentUserId", currentUserId.Value);
                return "SELECT QuizId FROM Quiz WHERE (IsDraft = 0 OR UserId = @CurrentUserId)";
            }
            return "SELECT QuizId FROM Quiz WHERE IsDraft = 0";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách ID Question được phép xem (Công khai hoặc Của mình)
        /// </summary>
        public static string BuildQueryAllowedQuestionIds(Guid? currentUserId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            if (currentUserId.HasValue)
            {
                parameters.Add("CurrentUserId", currentUserId.Value);
                return "SELECT QuestionId FROM Question WHERE (AccessType = 0 OR UserId = @CurrentUserId)";
            }
            return "SELECT QuestionId FROM Question WHERE AccessType = 0";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách câu hỏi theo danh sách QuestionId
        /// </summary>
        public static string BuildQueryQuestionsByIds(List<Guid> questionIds, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("Ids", questionIds);
            return "SELECT * FROM Question WHERE QuestionId IN @Ids";
        }
    }
}
