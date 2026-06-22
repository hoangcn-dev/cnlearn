using HoangCN.Core.BL.Attributes.AuthAction;
using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Enums;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoangCN.MainSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : CRUDController<Role>
    {
        private readonly IRoleService _roleService;

        public RolesController( 
            IRoleService roleService) : base(roleService)
        {
            _roleService = roleService;
        }

        protected override void ConfigurePolicies(AuthActionPolicyBuilder builder)
        {
            builder.Protect(nameof(GetAll), nameof(RoleNames.Admin));
            builder.Protect(nameof(GetById), nameof(RoleNames.Admin));
            builder.Protect(nameof(GetPaging), nameof(RoleNames.Admin));
            builder.Protect(nameof(Insert), nameof(RoleNames.Admin));
            builder.Protect(nameof(Update), nameof(RoleNames.Admin));
            builder.Protect(nameof(Delete), nameof(RoleNames.Admin));
        }
    }
}
