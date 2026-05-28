using Dapper;
using HoangCN.DL.Interfaces;
using MySqlConnector;
using System.Data;

namespace MISA.CukCuk.DL.Base
{
    /// <summary>
    /// Lớp triển khai cho giao diện cơ sở tầng dữ liệu
    /// </summary>
    public class BaseDL : IBaseDL
    {
        private readonly string _connectionString;
        private MySqlConnection? _connection;
        private MySqlTransaction? _transaction;

        public BaseDL(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Tạo giao dịch
        /// </summary>
        public async Task StartTransaction()
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_connectionString);
            }

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            if (_transaction == null)
            {
                _transaction = await _connection.BeginTransactionAsync();
            }
        }

        /// <summary>
        /// Xác nhận giao dịch
        /// </summary>
        /// <param name="transaction"></param>
        public async Task CommitTransaction()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        /// <summary>
        /// Rollback lại giao dịch
        /// </summary>
        public async Task RollbackTransaction()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public async Task<int> ExecuteCommandText(string command, DynamicParameters? parameters = null)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Không thể thực thi câu lệnh ghi dữ liệu ngoài phạm vi giao dịch (Transaction).");
            }

            var conn = _transaction.Connection;
            var res = await conn.ExecuteAsync(
                sql: command,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.Text);
            return res;
        }

        /// <summary>
        /// Thực thi câu lệnh truy vấn dữ liệu
        /// </summary>
        public async Task<IEnumerable<TRow>> ExecuteQueryText<TRow>(string query, DynamicParameters? parameters = null)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var res = await conn.QueryAsync<TRow>(query, parameters, commandType: CommandType.Text);
                return res;
            }
        }

        /// <summary>
        /// Thực thi câu lệnh truy vấn chỉ lấy dữ liệu trên ô đầu tiên
        /// </summary>
        public async Task<TResult?> ExecuteQueryToGetFirstResult<TResult>(string query, DynamicParameters? parameters = null)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var res = await conn.ExecuteScalarAsync<TResult>(query, parameters, commandType: CommandType.Text);
                return res;
            }
        }

        
    }
}
