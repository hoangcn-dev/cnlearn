namespace HoangCN.Common.Model.DTOs
{
    /// <summary>
    /// Lớp wrapper thông tin lỗi trả về khi xảy ra ngoại lệ
    /// </summary>
    public class ErrorResultDto
    {
        /// <summary>
        /// Thông báo dành cho Dev
        /// </summary>
        public string DevMsg { get; set; }

        /// <summary>
        /// Thông báo dành cho người dùng (hiển thị lên UI)
        /// </summary>
        public string UserMsg { get; set; }

        /// <summary>
        /// Đối tượng chứa thông tin chi tiết về lỗi (nếu có)
        /// </summary>
        public object MoreInfo { get; set; }

        /// <summary>
        /// Mã dùng cho truy vết đường đi request và lỗi
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public string ErrorCode { get; set; }
    }
}
