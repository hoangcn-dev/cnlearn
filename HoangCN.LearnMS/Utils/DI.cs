using HoangCN.Core.BL;
using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HoangCN.LearnMS.Utils
{
    /// <summary>
    /// Đăng ký Dependency Injection cho phân hệ Quản lý Học tập (LearnMS)
    /// </summary>
    public static class DI
    {
        /// <summary>
        /// Phương thức mở rộng để thêm các dịch vụ của module LearnMS vào container dịch vụ
        /// </summary>
        public static IServiceCollection AddLearnMS(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký HttpContextAccessor để hỗ trợ lấy thông tin HttpContext trong các Service
            services.AddHttpContextAccessor();

            // Đăng ký dịch vụ nghiệp vụ lõi Core.BL (BaseBL)
            services.AddCoreBL();

            // Đăng ký QuestionService với giao diện tùy chỉnh IQuestionService (vì có thêm phương thức Bulk import ngoài CRUD)
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IExamAttemptService, ExamAttemptService>();

            // Đăng ký QuestionCategoryService tùy chỉnh để hỗ trợ tự sinh SEO Slug khi tạo mới danh mục câu hỏi
            services.AddScoped<IBaseBL<QuestionCategory>, QuestionCategoryService>();

            // Đăng ký LearnMsUserService để xử lý nghiệp vụ thông tin người dùng riêng biệt
            services.AddScoped<ILearnMsUserService, LearnMsUserService>();

            // Đăng ký ExamService để xử lý lưu trữ đề thi động
            services.AddScoped<IExamService, ExamService>();

            // Đăng ký QuizService để xử lý phân quyền và nghiệp vụ bài kiểm tra
            services.AddScoped<IQuizService, QuizService>();

            // Đăng ký ExamSessionService cho tính năng chống gian lận
            services.AddScoped<IExamSessionService, ExamSessionService>();

            // Đăng ký Job kiểm tra phiên thi hết hạn
            services.AddHostedService<HoangCN.LearnMS.BackgroundServices.ExamSessionMonitorService>();

            return services;
        }
    }
}


