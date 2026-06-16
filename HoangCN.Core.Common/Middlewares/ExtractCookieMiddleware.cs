using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HoangCN.Core.Common.Middlewares
{
    /// <summary>
    /// Middleware trích xuất aToken từ Cookie và đưa vào Authorization Header dưới dạng Bearer Token
    /// </summary>
    public class ExtractCookieMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Cookies["aToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Authorization = "Bearer " + token;
            }
            await next(context);
        }
    }

    public static class ExtractCookieMiddlewareExtension
    {
        public static IServiceCollection AddExtractCookieMiddleware(this IServiceCollection services)
        {
            services.AddTransient<ExtractCookieMiddleware>();
            return services;
        }

        public static IApplicationBuilder UseExtractCookieMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExtractCookieMiddleware>();
        }
    }
}

