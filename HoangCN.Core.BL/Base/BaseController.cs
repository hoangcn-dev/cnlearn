using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HoangCN.Core.BL.Base
{
    [ApiController]
    [ServiceFilter(typeof(AuthActionFillter))]
    public class BaseController<TEntity> : ControllerBase where TEntity : BaseEntity
    {
        protected readonly IBaseBL<TEntity> _baseBL;
        private static readonly ConcurrentDictionary<Type, Dictionary<string, AuthActionSettings>> _defaultPoliciesCache = new();

        public BaseController(IBaseBL<TEntity> baseBL)
        {
            _baseBL = baseBL;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách cấu hình phân quyền của Controller
        /// </summary>
        public Dictionary<string, AuthActionSettings> GetPolicies()
        {
            var controllerType = this.GetType();
            var defaultPolicies = _defaultPoliciesCache.GetOrAdd(controllerType, type =>
            {
                var policies = new Dictionary<string, AuthActionSettings>();
                 
                // Quét tất cả các phương thức public instance trong controller và các lớp cha
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    var authAttr = method.GetCustomAttribute<AuthActionAttribute>(true);
                    if (authAttr != null)
                    {
                        // Sử dụng tên phương thức C# làm key chính để khớp chính xác với actionName trong filter
                        policies[method.Name] = new AuthActionSettings
                        {
                            IsEnabled = authAttr.IsEnabled,
                            Roles = authAttr.Roles
                        };
                    }
                }
                return policies;
            });

            // Clone lại cấu hình mặc định để mỗi instance Controller có thể tùy chỉnh riêng trong ConfigurePolicies
            var instancePolicies = defaultPolicies.ToDictionary(
                entry => entry.Key,
                entry => new AuthActionSettings
                {
                    IsEnabled = entry.Value.IsEnabled,
                    Roles = entry.Value.Roles
                }
            );

            ConfigurePolicies(new AuthActionPolicyBuilder(instancePolicies));
            return instancePolicies;
        }

        /// <summary>
        /// Cấu hình phân quyền cho các action của Controller
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void ConfigurePolicies(AuthActionPolicyBuilder builder)
        {

        }

        [HttpGet]
        [AuthAction]
        public virtual async Task<IActionResult> GetAll()
        {
            var res = await _baseBL.Get<TEntity>(GetRequest.GetAllRequest());
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpGet("{id}")]
        [AuthAction]
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
        [AuthAction]
        public virtual async Task<IActionResult> GetPaging([FromBody] GetRequest request)
        {
            var res = await _baseBL.Get<TEntity>(request);
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpPost]
        [AuthAction]
        public virtual async Task<IActionResult> Insert([FromBody] List<TEntity> entities)
        {
            await _baseBL.InsertAsync(entities);
            return Ok(ApiResponseDto.Success());
        }

        [HttpPut]
        [AuthAction]
        public virtual async Task<IActionResult> Update([FromBody] List<TEntity> entities)
        {
            await _baseBL.UpdateAsync(entities);
            return Ok(ApiResponseDto.Success());
        }

        [HttpPost("delete")]
        [AuthAction]
        public virtual async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            await _baseBL.DeleteAsync(request);
            return Ok(ApiResponseDto.Success());
        }
    }
}

