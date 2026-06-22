using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoangCN.Core.Common.Interfaces
{
    /// <summary>
    /// Giao diện dịch vụ HttpClient tiện ích để gọi API đồng bộ giữa các service
    /// </summary>
    public interface IHttpClientService
    {
        /// <summary>
        /// Thực hiện request GET bất đồng bộ
        /// </summary>
        Task<TResponse?> GetAsync<TResponse>(string url, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null);

        /// <summary>
        /// Thực hiện request POST bất đồng bộ
        /// </summary>
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null);

        /// <summary>
        /// Thực hiện request PUT bất đồng bộ
        /// </summary>
        Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null);

        /// <summary>
        /// Thực hiện request DELETE bất đồng bộ
        /// </summary>
        Task<TResponse?> DeleteAsync<TResponse>(string url, Dictionary<string, string>? headers = null);
    }
}
