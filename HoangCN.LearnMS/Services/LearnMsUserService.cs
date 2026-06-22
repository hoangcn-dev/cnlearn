using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Utils;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Dịch vụ xử lý nghiệp vụ cho thực thể LearnMsUser
    /// </summary>
    public class LearnMsUserService : BaseBL<LearnMsUser>, ILearnMsUserService
    {
        public LearnMsUserService(IBaseReadDL baseReadDL, IBaseWriteDL baseWriteDL, IHttpContextAccessor httpContextAccessor) 
            : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
        }

        public async Task<LearnMsUser> GetProfile(ClaimsPrincipal claims)
        {
            var userId = ClaimUtil.GetUserId(claims);
            if (userId == null)
            {
                throw new UnauthorizedException("Thông tin đăng nhập không hợp lệ");
            }

            var user = (await GetByCondition<LearnMsUser>(u => u.LearnMsUserId == userId)).FirstOrDefault();
            if (user == null)
            {
                user = await HandleInsertNewUserProfile(claims);
            }
            return user;
        }

        private async Task<LearnMsUser> HandleInsertNewUserProfile(ClaimsPrincipal claims)
        {
            var userId = ClaimUtil.GetUserId(claims);
            var fullName = ClaimUtil.GetByKey(claims, ClaimTypes.Name);
            var email = ClaimUtil.GetByKey(claims, ClaimTypes.Email);

            var newUser = new LearnMsUser
            {
                LearnMsUserId = userId.Value,
                FullName = fullName,
                Email = email,
                Biography = "Chưa có thông tin cá nhân",
                PhoneNumber = null,
                State = ModalState.Insert
            };

            await InsertEntities([newUser]);

            return newUser;
        }

        protected override async Task HandleBeforeUpdate(List<LearnMsUser> entities)
        {
            await base.HandleBeforeUpdate(entities);
            foreach (var entity in entities)
            {
                // Không cho phép cập nhật Email sau khi đã tạo tài khoản, vì đây là thông tin đồng bộ từ cổng Identity
                _baseWriteDL.SetChanged(entity, entity => entity.Email, false);
            }
        }
    }
}
