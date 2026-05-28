using HoangCN.BL.Interfaces;
using HoangCN.Common.Model.Entities;
using HoangCN.UserManagement.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoangCN.API.Controllers
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
