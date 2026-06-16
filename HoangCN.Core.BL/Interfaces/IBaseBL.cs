using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using System.Linq.Expressions;

namespace HoangCN.Core.BL.Interfaces
{
    /// <summary>
    /// Giao diện cơ sở cho các Service tầng nghiệp vụ sử dụng cơ chế EF Core (Write) và Dapper (Read)
    /// </summary>
    public interface IBaseBL<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Thêm mới danh sách thực thể
        /// </summary>
        Task InsertAsync(List<TEntity> entities);

        /// <summary>
        /// Cập nhật danh sách thực thể
        /// </summary>
        Task UpdateAsync(List<TEntity> entities);

        /// <summary>
        /// Xóa các thực thể theo danh sách ID
        /// </summary>
        Task DeleteAsync(DeleteRequest request);

        /// <summary>
        /// Lấy danh sách thực thể phân trang, sắp xếp và lọc động
        /// </summary>
        Task<ResultDto<TResult>> Get<TResult>(GetRequest request);

        /// <summary>
        /// Lấy danh sách thực thể phân trang, sắp xếp và lọc động kèm điều kiện lambda chỉ định
        /// </summary>
        Task<ResultDto<TResult>> Get<TResult>(GetRequest request, Expression<Func<TEntity, bool>> condition);

        /// <summary>
        /// Lấy danh sách thực thể theo điều kiện chỉ định (chỉ dùng cho nội bộ)
        /// </summary>
        Task<List<TResult>> GetByCondition<TResult>(Expression<Func<TEntity, bool>> condition);

        /// <summary>
        /// Lấy chi tiết thực thể theo ID (chỉ dùng cho nội bộ)
        /// </summary>
        Task<TResult?> GetById<TResult>(Guid id);
    }
}

