using Dapper;
using HoangCN.Core.DL.Interfaces;
using MySqlConnector;
using System.Data;

namespace HoangCN.Core.DL.Implementation
{
    /// <summary>
    /// Triển khai cơ chế đọc dữ liệu tối ưu hiệu năng sử dụng Dapper
    /// </summary>
    public class BaseReadDL : IBaseReadDL
    {
        private readonly string _connectionString;

        public BaseReadDL(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Chuỗi kết nối cơ sở dữ liệu đọc không được để trống.");
            }
            _connectionString = connectionString;
        }



        /// <summary>
        /// Thực thi truy vấn nhiều kết quả (Multiple Result Sets)
        /// </summary>
        public async Task<TResult> ExecuteQueryMultiple<TResult>(
            string query,
            Func<SqlMapper.GridReader, Task<TResult>> readerFunc,
            DynamicParameters? parameters = null)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                using (var multi = await conn.QueryMultipleAsync(query, parameters, commandType: CommandType.Text))
                {
                    return await readerFunc(multi);
                }
            }
        }

        /// <summary>
        /// Thực thi truy vấn lấy danh sách dữ liệu từ database đọc
        /// </summary>
        public async Task<IEnumerable<TRow>> ExecuteQueryText<TRow>(string query, DynamicParameters? parameters = null)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                return await conn.QueryAsync<TRow>(query, parameters, commandType: CommandType.Text);
            }
        }

        /// <summary>
        /// Thực thi truy vấn lấy kết quả ô đầu tiên (ví dụ COUNT) từ database đọc
        /// </summary>
        public async Task<TResult?> ExecuteQueryToGetFirstResult<TResult>(string query, DynamicParameters? parameters = null)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                return await conn.ExecuteScalarAsync<TResult>(query, parameters, commandType: CommandType.Text);
            }
        }

        /// <summary>
        /// Thực thi truy vấn lấy một dòng dữ liệu duy nhất từ database đọc
        /// </summary>
        public async Task<TRow?> ExecuteQuerySingle<TRow>(string query, DynamicParameters? parameters = null)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                return await conn.QueryFirstOrDefaultAsync<TRow>(query, parameters, commandType: CommandType.Text);
            }
        }
    }
}
