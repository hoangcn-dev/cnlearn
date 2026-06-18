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
                PhoneNumber = null
            };

            await InsertAsync([newUser]);

            return newUser;
        }

        /// <summary>
        /// Tiền xử lý trước khi lưu LearnMsUser
        /// Ghi đè phương thức này để bỏ qua cơ chế tự phát sinh ID mặc định của BaseBL, 
        /// do UserId được đồng bộ trực tiếp từ cổng Identity (MainSystem) sang.
        /// </summary>
        protected override async Task BeforeInsert(List<LearnMsUser> entities)
        {
            await base.BeforeInsert(entities);
            foreach (var entity in entities)
            {
                entity.CreatedBy = "System";
                entity.ModifiedBy = "System";
            }
        }

        protected override async Task BeforeUpdate(List<LearnMsUser> entities)
        {
            await base.BeforeUpdate(entities);
            foreach (var entity in entities)
            {
                // Không cho phép cập nhật Email sau khi đã tạo tài khoản, vì đây là thông tin đồng bộ từ cổng Identity
                _baseWriteDL.SetChanged(entity, entity => entity.Email, false);
            }
        }
    }
}
