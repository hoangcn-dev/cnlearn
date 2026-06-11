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
        /// Tạo câu lệnh SQL để lấy danh sách QuestionId theo danh mục
        /// </summary>
        public static string BuildQueryQuestionIdsByCategory(Guid catId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("CatId", catId);
            return "SELECT QuestionId FROM QuestionInCategory WHERE QuestionCategoryId = @CatId AND IsDeleted = 0";
        }

        /// <summary>
        /// Tạo câu lệnh SQL để lấy danh sách câu trả lời theo danh sách QuestionId
        /// </summary>
        public static string BuildQueryAnswersByQuestionIds(List<Guid> questionIds, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("Ids", questionIds);
            return "SELECT * FROM QuestionAnswer WHERE QuestionId IN @Ids AND IsDeleted = 0";
        }

        /// <summary>
        /// Tạo câu lệnh SQL để lấy danh sách liên kết danh mục theo danh sách QuestionId
        /// </summary>
        public static string BuildQueryCategoriesByQuestionIds(List<Guid> questionIds, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("Ids", questionIds);
            return "SELECT * FROM QuestionInCategory WHERE QuestionId IN @Ids AND IsDeleted = 0";
        }

        /// <summary>
        /// Tạo câu lệnh SQL để lấy danh sách câu trả lời theo một QuestionId
        /// </summary>
        public static string BuildQueryAnswersByQuestionId(Guid questionId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("Id", questionId);
            return "SELECT * FROM QuestionAnswer WHERE QuestionId = @Id AND IsDeleted = 0 ORDER BY OrderInList";
        }

        /// <summary>
        /// Tạo câu lệnh SQL để lấy danh sách liên kết danh mục theo một QuestionId
        /// </summary>
        public static string BuildQueryCategoriesByQuestionId(Guid questionId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("Id", questionId);
            return "SELECT * FROM QuestionInCategory WHERE QuestionId = @Id AND IsDeleted = 0 ORDER BY OrderInList";
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
                FROM UserAttempt 
                WHERE UserId = @UserId AND AttemptType = 'question' AND QuestionId IS NOT NULL AND IsDeleted = 0
                UNION
                SELECT DISTINCT d.QuestionId 
                FROM UserAttemptDetail d
                INNER JOIN UserAttempt a ON d.UserAttemptId = a.UserAttemptId
                WHERE a.UserId = @UserId AND a.AttemptType != 'question' AND a.IsDeleted = 0 AND d.IsDeleted = 0";
        }

        /// <summary>
        /// Tạo câu lệnh SQL lấy danh sách ID câu hỏi đã lưu của người dùng
        /// </summary>
        public static string BuildQuerySavedQuestionIds(Guid userId, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            return "SELECT DISTINCT QuestionId FROM UserSavedQuestion WHERE UserId = @UserId AND IsDeleted = 0";
        }
    }
}
