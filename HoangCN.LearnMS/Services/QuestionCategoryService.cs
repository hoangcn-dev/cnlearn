using HoangCN.BL.Base;
using HoangCN.Common.Base;
using HoangCN.Common.Enums;
using HoangCN.Common.Model.Entities;
using HoangCN.Common.Utils;
using HoangCN.DL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Triển khai dịch vụ tùy chỉnh cho danh mục câu hỏi (QuestionCategory) để tự động sinh SEO slug khi tạo danh mục
    /// </summary>
    public class QuestionCategoryService : BaseBL<QuestionCategory>
    {
        public QuestionCategoryService(IBaseDL baseDL) : base(baseDL)
        {
        }

        protected override async Task BeforeSave(List<QuestionCategory> entities)
        {
            foreach (var entity in entities)
            {
                if (entity == null) continue;

                // Nếu Slug rỗng hoặc null, tự động sinh slug từ Name
                if (string.IsNullOrWhiteSpace(entity.Slug))
                {
                    entity.Slug = SlugUtil.GenerateSlug(entity.Name);
                }
                else
                {
                    // Nếu đã nhập slug thủ công, vẫn chuẩn hóa bằng SlugUtil để làm sạch chuỗi
                    entity.Slug = SlugUtil.GenerateSlug(entity.Slug);
                }
            }

            await base.BeforeSave(entities);
        }
    }
}
