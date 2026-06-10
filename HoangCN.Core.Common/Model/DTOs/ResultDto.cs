namespace HoangCN.Common.Model.DTOs
{
    /// <summary>
    /// Lớp Wrapper cho danh sách đã phân trang
    /// </summary>
    public class ResultDto<TItem>
    {
        /// <summary>
        /// Tổng số lượng tìm thấy (khi chưa phân trang)
        /// </summary>
        public int Total { get; set; } = 0;

        /// <summary>
        /// Số trang
        /// </summary>
        public int Page { get; set; } = 0;

        /// <summary>
        /// Kích thước phân trang
        /// </summary>
        public int Size { get; set; } = 1;

        /// <summary>
        /// Số trang tối đa
        /// </summary>
        public int TotalPages => Total / Size + (Total % Size > 0 ? 1 : 0);

        /// <summary>
        /// Danh sách phần tử 
        /// </summary>
        public List<TItem> Items { get; set; } = [];
    }
}
