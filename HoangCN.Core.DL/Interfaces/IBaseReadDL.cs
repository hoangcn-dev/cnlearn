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

        /// <summary>
        /// Thực thi truy vấn nhiều kết quả (Multiple Result Sets) sử dụng Dapper
        /// </summary>
        /// <typeparam name="TResult">Kiểu dữ liệu kết quả trả về sau khi map</typeparam>
        /// <param name="query">Chuỗi truy vấn SQL chứa nhiều câu lệnh phân tách bằng dấu chấm phẩy</param>
        /// <param name="readerFunc">Callback dùng để đọc dữ liệu từ GridReader</param>
        /// <param name="parameters">Bộ tham số truyền vào truy vấn</param>
        Task<TResult> ExecuteQueryMultiple<TResult>(
            string query,
            Func<SqlMapper.GridReader, Task<TResult>> readerFunc,
            DynamicParameters? parameters = null);
    }
}
