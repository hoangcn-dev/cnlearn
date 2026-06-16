namespace HoangCN.MainSystem.Interfaces
{
    /// <summary>
    /// Giao diện dịch vụ tương tác với Redis Cache
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// Lưu giá trị vào Redis kèm thời gian sống (TTL)
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// Lấy giá trị từ Redis theo Key
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Xóa giá trị khỏi Redis theo Key
        /// </summary>
        Task<bool> DeleteAsync(string key);
    }
}
