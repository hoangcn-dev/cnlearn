namespace HoangCN.Common.Model.DTOs
{
    /// <summary>
    /// Lớp wrapper cho phản hồi từ API
    /// </summary>
    public class ApiResponseDto
    {
        /// <summary>
        /// Đã thành công hay chưa
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Dữ liệu phản hồi
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Factory method cho phản hồi 200
        /// </summary>
        public static ApiResponseDto Success(object? data = null)
        {
            return new ApiResponseDto
            {
                IsSuccess = true,
                Data = data,
            };
        }

        /// <summary>
        /// Factory method cho phản hồi lỗi
        /// </summary>
        public static ApiResponseDto Failure(ErrorResultDto error)
        {
            return new ApiResponseDto
            {
                IsSuccess = false,
                Data = error,
            };
        }
    }
}
