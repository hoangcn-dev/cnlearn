using HoangCN.Core.Common.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HoangCN.Core.DL.Interfaces
{
    /// <summary>
    /// Giao diện cơ sở cho cơ chế ghi dữ liệu sử dụng EF Core
    /// </summary>
    public interface IBaseWriteDL
    {
        /// <summary>
        /// Bắt đầu một Transaction mới
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Xác nhận lưu các thay đổi và kết thúc Transaction thành công
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Hủy bỏ toàn bộ thay đổi trong Transaction khi xảy ra lỗi
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Lưu danh sách entity tự động đệ quy và đồng bộ khóa ngoại
        /// </summary>
        Task<List<TEntity>> SaveEntities<TEntity, TParentEntity>(List<TEntity> entities, TParentEntity? parent = null)
            where TParentEntity : BaseEntity
            where TEntity : BaseEntity;

        /// <summary>
        /// Cập nhật trạng thái thay đổi của entity trong DbContext (theo dõi hoặc không theo dõi)
        /// </summary>
        void SetChanged<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertySelector, bool isChanged) where TEntity : BaseEntity;
    }
}

