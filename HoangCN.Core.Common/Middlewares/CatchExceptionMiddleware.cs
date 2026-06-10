using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace HoangCN.Core.Common.Middlewares
{
    public class CatchExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                
                await next(context);
            }
            catch (BadRequestException ex)
            {
                await HandleException(context, ex, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                await HandleException(context, ex, StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await HandleException(context, ex, StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (NotFoundException ex)
            {
                await HandleException(context, ex, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex, StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi, vui lòng liên hệ admin để được hỗ trợ");
            }
        }

        private async Task HandleException(HttpContext context, Exception ex, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(ApiResponseDto.Failure(new ErrorResultDto
            {
                DevMsg = ex.Message,
                UserMsg = message,
                MoreInfo = ex.Data,
            })));
        }
    }

    public static class CatchExceptionMiddlewareExtension
    {
        public static IServiceCollection AddCatchExceptionMiddleware(this IServiceCollection services)
        {
            services.AddTransient<CatchExceptionMiddleware>();
            return services;
        }
    }
}

