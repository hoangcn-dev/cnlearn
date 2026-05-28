using Dapper;

namespace HoangCN.DL.Interfaces
{
    /// <summary>
    /// Giao diện cơ sở cho các dịch vụ tầng dữ liệu
    /// </summary>
    public interface IBaseDL
    {
        /// <summary>
        /// Tạo một phiên giao dịch mới cho các thao tác thay đổi cơ sở dữ liệu
        /// </summary>
        Task StartTransaction();

        /// <summary>
        /// Xác nhận giao dịch cho các thao tác thay đổi cơ sở dữ liệu
        /// </summary>
        Task CommitTransaction();

        /// <summary>
        /// Loại bỏ giao dịch lỗi
        /// </summary>
        Task RollbackTransaction();

        /// <summary>
        /// Thực thi câu lệnh thay đổi cơ sơ dữ liệu
        /// </summary>
        Task<int> ExecuteCommandText(string command, DynamicParameters? parameters = null);

        /// <summary>
        /// Thực thi truy vấn để lấy dữ liệu từ DB
        /// </summary>
        Task<IEnumerable<TRow>> ExecuteQueryText<TRow>(string query, DynamicParameters? parameters = null);

        /// <summary>
        /// Thực thi truy vấn để lấy dữ liệu từ DB nhưng chỉ lấy kết quả ở ô đầu tiên
        /// </summary>
        Task<TResult?> ExecuteQueryToGetFirstResult<TResult>(string query, DynamicParameters? parameters = null);
    }
}
