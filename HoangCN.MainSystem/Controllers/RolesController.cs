using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoangCN.MainSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(RoleNames.Admin))]
    public class RolesController : BaseController<Role>
    {
        public RolesController(IBaseBL<Role> baseBL) : base(baseBL)
        {
        }
    }
}
