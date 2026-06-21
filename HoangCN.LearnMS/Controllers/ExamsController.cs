using HoangCN.Core.BL.Attributes.AuthAction;
using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý đề thi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : CRUDController<Exam>
    {
        private readonly IExamService _examService;

        public ExamsController(
            IExamService examService) : base(examService)
        {
            _examService = examService;
        } 
    }
}
