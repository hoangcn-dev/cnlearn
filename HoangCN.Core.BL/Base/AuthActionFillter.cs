using HoangCN.Core.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HoangCN.Core.BL.Base
{
    public class AuthActionFillter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller;
            var controllerType = controller.GetType();

            // 1. Kiểm tra xem controller hiện tại có kế thừa từ BaseController<> hay không
            if (!IsSubclassOfRawGeneric(typeof(BaseController<>), controllerType))
            {
                return next();
            }

            // 2. Lấy tên Action Method thực tế trong C# (ví dụ: "GetAll", "Insert")
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor == null)
            {
                throw new InvalidOperationException($"The BaseController action with name {actionDescriptor} does not exist");
            }
            var actionName = actionDescriptor.ActionName;

            // 3. Kiểm tra settings để xác định quyền truy cập
            dynamic dynamicController = controller;
            Dictionary<string, AuthActionSettings> policies = dynamicController.GetPolicies();
            if (!policies.TryGetValue(actionName, out var settings))
            {
                // Nếu không có cấu hình phân quyền (không có AuthorizeAction attribute), cho phép tiếp tục bình thường
                return next();
            }

            // Chặn truy cập nếu endpoint bị vô hiệu hóa
            if (!settings.IsEnabled)
            {
                throw new ForbiddenException("This endpoint is disabled");
            }

            // Cho phép tất cả user đã có tài khoản + đăng nhập sử dụng
            var user = context.HttpContext.User;
            if (user == null || user.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedException("This endpoint require login to use");
            }
            if (settings.Roles == null || settings.Roles.Length == 0)
            {
                return next();
            }

            // Chỉ cho phép user có quyền
            if (!settings.Roles.Any(role => user.IsInRole(role)))
            {
                throw new ForbiddenException("You do not have permission to access this endpoint");
            }

            return next();
        }

        private bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
