using Dapper;

namespace HoangCN.Core.DL.Interfaces
{
    /// <summary>
    /// Giao diện cơ sở cho cơ chế đọc dữ liệu sử dụng Dapper
    /// </summary>
    public interface IBaseReadDL
    {
        /// <summary>
        /// Thực thi truy vấn lấy danh sách dữ liệu từ database đọc
        /// </summary>
        Task<IEnumerable<TRow>> ExecuteQueryText<TRow>(string query, DynamicParameters? parameters = null);

        /// <summary>
        /// Thực thi truy vấn lấy kết quả ô đầu tiên (ví dụ COUNT) từ database đọc
        /// </summary>
        Task<TResult?> ExecuteQueryToGetFirstResult<TResult>(string query, DynamicParameters? parameters = null);
    }
}
