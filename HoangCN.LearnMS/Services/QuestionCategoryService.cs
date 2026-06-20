using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Entities;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Triển khai dịch vụ tùy chỉnh cho danh mục câu hỏi (QuestionCategory) để tự động sinh SEO slug khi tạo danh mục
    /// </summary>
    public class QuestionCategoryService : BaseBL<QuestionCategory>
    {
        private readonly IBaseBL<Question> _questionService;

        public QuestionCategoryService(
            IBaseReadDL baseReadDL, 
            IBaseWriteDL baseWriteDL, 
            IHttpContextAccessor httpContextAccessor,
            IBaseBL<Question> questionService) 
            : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
            _questionService = questionService;
        }

        protected override async Task BeforeInsert(List<QuestionCategory> entities)
        {
            await base.BeforeInsert(entities);
            GenerateSlugForCategories(entities);
            await ValidateParentCategories(entities);
        }

        protected override async Task BeforeUpdate(List<QuestionCategory> entities)
        {
            await base.BeforeUpdate(entities);
            GenerateSlugForCategories(entities);
            await ValidateParentCategories(entities);
        }

        private void GenerateSlugForCategories(List<QuestionCategory> entities)
        {
            foreach (var entity in entities)
            {
                if (entity == null) continue;

                // Nếu Slug rỗng hoặc null, tự động sinh slug từ Name
                if (string.IsNullOrWhiteSpace(entity.QuestionCategorySlug))
                {
                    entity.QuestionCategorySlug = SlugUtil.GenerateSlug(entity.QuestionCategoryName);
                }
                else
                {
                    // Nếu đã nhập slug thủ công, vẫn chuẩn hóa bằng SlugUtil để làm sạch chuỗi
                    entity.QuestionCategorySlug = SlugUtil.GenerateSlug(entity.QuestionCategorySlug);
                }
            }
        }

        private async Task ValidateParentCategories(List<QuestionCategory> entities)
        {
            var parentIds = entities
                .Select(c => c.ParentId)
                .Where(id => id.HasValue && id.Value != Guid.Empty)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            if (parentIds.Count == 0) return;

            // Kiểm tra xem các ParentId này có đang chứa bất kỳ câu hỏi nào hay không (đáp ứng điều kiện IsDeleted = false)
            var questions = await _questionService.GetByCondition<Question>(q => parentIds.Contains(q.QuestionCategoryId) && !q.IsDeleted);

            if (questions.Count > 0)
            {
                var violatedParentIds = questions
                    .Select(q => q.QuestionCategoryId)
                    .Distinct()
                    .ToList();

                // Lấy tên các danh mục cha bị vi phạm
                var parentCategories = await GetByCondition<QuestionCategory>(c => violatedParentIds.Contains(c.QuestionCategoryId));
                var categoryNames = parentCategories.Select(c => c.QuestionCategoryName).ToList();

                var violatedNames = string.Join(", ", categoryNames);
                throw new BadRequestException($"Không thể thiết lập danh mục con cho danh mục: {violatedNames} vì danh mục này đang chứa câu hỏi trực tiếp.");
            }
        }
    }
}


