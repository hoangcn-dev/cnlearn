using HoangCN.Core.Common.Model.DTOs;
using HoangCN.MainSystem.Interfaces;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;

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

