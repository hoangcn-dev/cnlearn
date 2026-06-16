using HoangCN.Core.BL.Base;
using HoangCN.Core.DL.Interfaces;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HoangCN.MainSystem.Services
{
    public class RoleService : BaseBL<Role>, IRoleService
    {
        public RoleService(IBaseReadDL baseReadDL, IBaseWriteDL baseWriteDL, IHttpContextAccessor httpContextAccessor) 
            : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
        }
    }
}
