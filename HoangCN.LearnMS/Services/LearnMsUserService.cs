using HoangCN.Core.BL.Base;
using HoangCN.Core.DL.Interfaces;
using HoangCN.Core.Common.Enums;
using HoangCN.LearnMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HoangCN.LearnMS.Interfaces;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Dịch vụ xử lý nghiệp vụ cho thực thể LearnMsUser
    /// </summary>
    public class LearnMsUserService : BaseBL<LearnMsUser>, ILearnMsUserService
    {
        public LearnMsUserService(IBaseReadDL baseReadDL, IBaseWriteDL baseWriteDL) 
            : base(baseReadDL, baseWriteDL)
        {
        }

        /// <summary>
        /// Tiền xử lý trước khi lưu LearnMsUser
        /// Ghi đè phương thức này để bỏ qua cơ chế tự phát sinh ID mặc định của BaseBL, 
        /// do UserId được đồng bộ trực tiếp từ cổng Identity (MainSystem) sang.
        /// </summary>
        protected override async Task BeforeSave(List<LearnMsUser> entities)
        {
            foreach (var entity in entities)
            {
                if (entity == null || entity.State == ModelState.Delete) continue;

                if (entity.State == ModelState.None)
                {
                    // Kiểm tra xem User đã tồn tại trong database LearnMS hay chưa bằng Dapper
                    var existingUsers = await GetByCondition<LearnMsUser>(x => x.LearnMsUserId == entity.LearnMsUserId);
                    var existingUser = existingUsers.FirstOrDefault();
                    if (existingUser == null)
                    {
                        entity.State = ModelState.Insert;
                        entity.CreatedBy = "System";
                        entity.CreatedDate = DateTime.Now;
                        entity.ModifiedDate = entity.CreatedDate;
                    }
                    else
                    {
                        entity.State = ModelState.Update;
                        entity.ModifiedBy = "System";
                        entity.ModifiedDate = DateTime.Now;

                        // Chặn việc sửa đổi Email và UserId ở mức logic
                        entity.Email = existingUser.Email;
                    }
                }
            }

            // Gọi các nghiệp vụ kiểm tra tính hợp lệ cơ bản khác
            await ValidateBulk(entities);
        }
    }
}
