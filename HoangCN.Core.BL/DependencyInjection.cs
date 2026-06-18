using Microsoft.Extensions.DependencyInjection;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Interfaces;
using HoangCN.Core.Common.Services;

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

            // Đăng ký HttpClient và dịch vụ HttpClientService tiện ích dùng chung
            services.AddHttpClient();
            services.AddScoped<IHttpClientService, HttpClientService>();

            return services;
        }
    }
}
