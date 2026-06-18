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
    [Authorize(Roles = nameof(RoleNames.Admin))]
    public class EmailTemplatesController : BaseController<EmailTemplate>
    {
        public EmailTemplatesController(IBaseBL<EmailTemplate> baseBL) : base(baseBL)
        {
        }
    }
}

