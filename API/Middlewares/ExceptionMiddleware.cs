
using API.Models;
using Core.Exceptions;
using Core.Utilities;
using System.Text.Json;

namespace API.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await WriteResponseAsync(context, ApiResponse.Error(ex.Message));
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await WriteResponseAsync(context, ApiResponse.Error(ex.Message));
            }
            catch (ForbiddenException ex)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await WriteResponseAsync(context, ApiResponse.Error(ex.Message));
            }
            catch (UnauthorizedException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await WriteResponseAsync(context, ApiResponse.Error(ex.Message));
            }
            catch (ServerErrorException ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await WriteResponseAsync(context, ApiResponse.Error(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError("Unknown error: " + ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await WriteResponseAsync(context, ApiResponse.Error(StringUtil.ApiMessages.UnknownError));
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, object response)
        {
            context.Response.ContentType = "application/json";
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
