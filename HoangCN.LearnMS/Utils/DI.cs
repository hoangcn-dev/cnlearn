using HoangCN.BL.Interfaces;
using HoangCN.Common.Model.Entities;
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
            // Đăng ký QuestionService với giao diện tùy chỉnh IQuestionService (vì có thêm phương thức Bulk import ngoài CRUD)
            services.AddScoped<IQuestionService, QuestionService>();

            // Đăng ký QuestionCategoryService tùy chỉnh để hỗ trợ tự sinh SEO Slug khi tạo mới danh mục câu hỏi
            services.AddScoped<IBaseBL<QuestionCategory>, QuestionCategoryService>();

            return services;
        }
    }
}
