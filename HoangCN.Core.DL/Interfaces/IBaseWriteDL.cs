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
        /// Lấy IQueryable để thực hiện truy vấn với EF Core (hỗ trợ LINQ, Include)
        /// </summary>
        DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class;

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
        /// Lưu các thay đổi hiện tại vào database ghi
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Ghi danh sách tự phát hiện thêm, sửa hoặc xóa
        /// </summary>
        Task SaveRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        /// <summary>
        /// Thêm danh sách entity vào database
        /// </summary>
        Task InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        /// <summary>
        /// Cập nhật danh sách entity trong database
        /// </summary>
        Task UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        /// <summary>
        /// Xóa danh sách entity (hỗ trợ cả xóa mềm/xóa cứng tùy thuộc vào IsDeleted)
        /// </summary>
        Task DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        /// <summary>
        /// Cập nhật trạng thái thay đổi của entity trong DbContext (theo dõi hoặc không theo dõi)
        /// </summary>
        void SetChanged<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertySelector, bool isChanged) where TEntity : BaseEntity;
    }
}

