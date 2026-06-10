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
        /// Lưu danh sách thực thể dựa trên trạng thái State (Thêm/Sửa/Xóa)
        /// </summary>
        Task Save(List<TEntity> entities);

        /// <summary>
        /// Xóa các thực thể theo danh sách ID
        /// </summary>
        Task Delete(DeleteRequest request);

        /// <summary>
        /// Lấy danh sách thực thể phân trang, sắp xếp và lọc động
        /// </summary>
        Task<ResultDto<TResult>> Get<TResult>(GetRequest request);

        /// <summary>
        /// Lấy thông tin thực thể bằng ID
        /// </summary>
        Task<TResult?> GetById<TResult>(Guid id);

        /// <summary>
        /// Lấy danh sách thực thể theo điều kiện chỉ định
        /// </summary>
        Task<List<TResult>> GetByCondition<TResult>(Expression<Func<TEntity, bool>> condition);
    }
}

