using HoangCN.Core.BL.Base;
using HoangCN.Core.DL.Interfaces;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.BL.Utils;
using HoangCN.LearnMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HoangCN.LearnMS.Interfaces;

using Microsoft.AspNetCore.Http;

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
                entity.ModifiedBy = "System";
            }
        }
    }
}
