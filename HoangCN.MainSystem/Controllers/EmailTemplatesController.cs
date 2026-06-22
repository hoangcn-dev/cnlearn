using HoangCN.Core.BL.Attributes.AuthAction;
using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Enums;
using HoangCN.MainSystem.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HoangCN.MainSystem.Controllers
{
    /// <summary>
    /// API Quản lý các mẫu Template Email - Chỉ dành riêng cho Admin
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTemplatesController : CRUDController<EmailTemplate>
    {
        public EmailTemplatesController(IBaseBL<EmailTemplate> baseBL) : base(baseBL)
        {
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

