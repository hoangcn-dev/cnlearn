using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Controllers
{
    [ApiController]
    public class BaseController<TEntity> : ControllerBase where TEntity : BaseEntity
    {
        protected readonly IBaseBL<TEntity> _baseBL;

        public BaseController(IBaseBL<TEntity> baseBL)
        {
            _baseBL = baseBL;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var res = await _baseBL.Get<TEntity>(GetRequest.GetAllRequest());
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var res = await _baseBL.Get<TEntity>(GetRequest.GetByIdRequest(id));
            if (res == null || res.Items.Count == 0)
            {
                throw new NotFoundException("Đối tượng không tồn tại");
            }
            return Ok(ApiResponseDto.Success(res.Items[0]));
        }

        [HttpPost("paging")]
        public virtual async Task<IActionResult> GetPaging([FromBody] GetRequest request)
        {
            var res = await _baseBL.Get<TEntity>(request);
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Save([FromBody] List<TEntity> entities)
        {
            await _baseBL.Save(entities);
            return Ok(ApiResponseDto.Success());
        }

        [HttpPost("delete")] 
        public virtual async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            await _baseBL.Delete(request);
            return Ok(ApiResponseDto.Success());
        }
    }
}
