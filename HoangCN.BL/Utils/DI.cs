using HoangCN.BL.Base;
using HoangCN.BL.Interfaces;
using HoangCN.DL.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HoangCN.BL.Utils
{
    /// <summary>
    /// Lớp cấu hình Dependency Injection cho tầng nghiệp vụ (BL)
    /// </summary>
    public static class DI
    {
        public static IServiceCollection AddBL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDL(configuration);
            services.AddScoped(typeof(IBaseBL<>), typeof(BaseBL<>));
            return services;
        }
    }
}
