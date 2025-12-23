using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Realtime
{
    public class AppHub : Hub
    {
        protected string? GetQueryParam(string key)
        {
            return Context.GetHttpContext()?.Request.Query[key];
        }

        protected ClaimsPrincipal? GetClaimsPrincipal()
        {
            return Context.User;
        }
    }
}
