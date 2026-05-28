using HoangCN.Common.Base;
using HoangCN.Common.Model.DTOs;
using HoangCN.Common.Model.Requests;
using System.Linq.Expressions;

namespace HoangCN.BL.Interfaces
{
    /// <summary>
    /// Giao diện cơ sở cho các Service tầng nghiệp vụ
    /// </summary>
    public interface IBaseBL<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Lưu trạng thái entity dựa trên state của chúng (Thêm/Sửa/Xóa)
        /// </summary>
        Task Save(List<TEntity> entities);

        /// <summary>
        /// Xóa các entity có ID nằm trong danh sách
        /// </summary>
        Task Delete(DeleteRequest request);

        /// <summary>
        /// Lấy các entity có ID nằm trong danh sách, trả về mapping theo TResult
        /// </summary>
        Task<ResultDto<TResult>> Get<TResult>(GetRequest request);

        Task<TResult> GetById<TResult>(Guid id);
        Task<List<TResult>> GetByCondition<TResult>(Expression<Func<TEntity, bool>> condition);
    }
}
