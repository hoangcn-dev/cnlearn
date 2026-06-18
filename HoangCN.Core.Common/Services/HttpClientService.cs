using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HoangCN.Core.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HoangCN.Core.Common.Services
{
    /// <summary>
    /// Dịch vụ HttpClient tiện ích để gọi API đồng bộ giữa các service
    /// </summary>
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public HttpClientService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Tạo HttpClient và tự động đính kèm Token xác thực + Cookie từ HttpContext hiện tại
        /// </summary>
        private HttpClient CreateClientWithContextHeaders(Dictionary<string, string>? customHeaders = null)
        {
            var client = _httpClientFactory.CreateClient();

            // Lấy HttpContext hiện tại
            var currentContext = _httpContextAccessor.HttpContext;
            if (currentContext != null)
            {
                // Chuyển tiếp Token Authorization
                if (currentContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authHeader.ToString());
                }

                // Chuyển tiếp Cookie
                if (currentContext.Request.Headers.TryGetValue("Cookie", out var cookieHeader))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", cookieHeader.ToString());
                }
            }

            // Đính kèm các tiêu đề tùy chỉnh nếu có
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return client;
        }

        /// <summary>
        /// Xử lý phản hồi từ API, giải tuần tự hóa JSON hoặc ném ngoại lệ nếu có lỗi xảy ra
        /// </summary>
        private async Task<TResponse?> HandleResponseAsync<TResponse>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Gọi API thất bại với mã trạng thái {response.StatusCode}. Chi tiết phản hồi: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return default;
            }

            return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
        }

        public async Task<TResponse?> GetAsync<TResponse>(string url, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null)
        {
            if (queryParams != null && queryParams.Count > 0)
            {
                var sb = new StringBuilder(url);
                sb.Append(url.Contains("?") ? "&" : "?");
                foreach (var param in queryParams)
                {
                    sb.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}&");
                }
                url = sb.ToString().TrimEnd('&');
            }

            var client = CreateClientWithContextHeaders(headers);
            var response = await client.GetAsync(url);
            return await HandleResponseAsync<TResponse>(response);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null)
        {
            var client = CreateClientWithContextHeaders(headers);
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            return await HandleResponseAsync<TResponse>(response);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null)
        {
            var client = CreateClientWithContextHeaders(headers);
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, content);
            return await HandleResponseAsync<TResponse>(response);
        }

        public async Task<TResponse?> DeleteAsync<TResponse>(string url, Dictionary<string, string>? headers = null)
        {
            var client = CreateClientWithContextHeaders(headers);
            var response = await client.DeleteAsync(url);
            return await HandleResponseAsync<TResponse>(response);
        }
    }
}
