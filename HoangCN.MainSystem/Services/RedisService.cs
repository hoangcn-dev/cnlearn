using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using HoangCN.MainSystem.Interfaces;
using HoangCN.Core.Common.Utils;
using HoangCN.MainSystem.Utils;

namespace HoangCN.MainSystem.Services
{
    /// <summary>
    /// Triển khai dịch vụ Redis Cache lưu trữ mật khẩu tạm thời và các khóa đệm
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IDatabase? _database;
        private readonly ILogger<RedisService> _logger;
        private readonly string _connectionString;

        public RedisService(IConfiguration configuration, ILogger<RedisService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = EnvUtil.GetValue(EnvKeys.CONNNECTION_STRING_REDIS);

            try
            {
                var options = ConfigurationOptions.Parse(_connectionString);
                
                // Nạp mật khẩu từ biến môi trường nếu trong connection string chưa cấu hình mật khẩu
                //var envPassword = Environment.GetEnvironmentVariable("REDIS_CONNECTION_PASSWORD");
                //if (string.IsNullOrEmpty(options.Password) && !string.IsNullOrEmpty(envPassword))
                //{
                //    options.Password = envPassword;
                //}

                options.AbortOnConnectFail = false; // Tránh treo ứng dụng nếu Redis chưa chạy
                options.ConnectTimeout = 5000;
                
                var multiplexer = ConnectionMultiplexer.Connect(options);
                _database = multiplexer.GetDatabase();
                _logger.LogInformation("Đang kết nối Redis với ConnectionString: {ConnectionString}, Ssl: {Ssl}", _connectionString, options.Ssl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể kết nối tới máy chủ Redis tại: {ConnectionString}", _connectionString);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }
            
            if (_database == null)
            {
                _logger.LogWarning("Redis Connection is not established. Cannot set key: {Key}", key);
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, json, expiry.HasValue ? expiry.Value : default);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu dữ liệu vào Redis với Key: {Key}", key);
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            if (_database == null)
            {
                _logger.LogWarning("Redis Connection is not established. Cannot get key: {Key}", key);
                return default;
            }

            try
            {
                var value = await _database.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu từ Redis với Key: {Key}", key);
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            if (_database == null)
            {
                _logger.LogWarning("Redis Connection is not established. Cannot delete key: {Key}", key);
                return false;
            }

            try
            {
                return await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa Key khỏi Redis: {Key}", key);
                return false;
            }
        }
    }
}
