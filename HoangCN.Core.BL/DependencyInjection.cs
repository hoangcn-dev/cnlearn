using Microsoft.Extensions.DependencyInjection;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Base;

namespace HoangCN.Core.BL
{
    /// <summary>
    /// Các phương thức mở rộng để tiêm phụ thuộc các dịch vụ nghiệp vụ
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Đăng ký dịch vụ nghiệp vụ lõi Core.BL (BaseBL)
        /// </summary>
        public static IServiceCollection AddCoreBL(this IServiceCollection services)
        {
            // Đăng ký dịch vụ nghiệp vụ cơ sở dưới dạng Open Generic
            services.AddScoped(typeof(IBaseBL<>), typeof(BaseBL<>));

            // Đăng ký BaseControllerAuthorizeFillter để sử dụng trong [ServiceFilter]
            services.AddScoped<AuthActionFillter>();

            return services;
        }
    }
}
