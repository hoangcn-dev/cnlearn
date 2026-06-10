using Microsoft.EntityFrameworkCore;
using HoangCN.Core.Common.Base;

namespace HoangCN.Core.DL.Interfaces
{
    /// <summary>
    /// Giao diện cơ sở cho cơ chế ghi dữ liệu sử dụng EF Core
    /// </summary>
    public interface IBaseWriteDL
    {
        /// <summary>
        /// Đối tượng DbContext của EF Core dùng để thao tác trực tiếp nếu cần
        /// </summary>
        DbContext Context { get; }

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
        /// Lưu danh sách entity dựa trên trạng thái State của từng đối tượng
        /// </summary>
        Task SaveEntitiesAsync<TEntity>(List<TEntity> entities) where TEntity : BaseEntity;
    }
}

