namespace HoangCN.Common.Enums
{
    /// <summary>
    /// Quản lý trạng thái của entity
    /// </summary>
    public enum ModelState
    {
        /// <summary>
        /// Không làm gì
        /// </summary>
        None = 0,

        /// <summary>
        /// Cần thêm mới
        /// </summary>
        Insert = 1,

        /// <summary>
        /// Cần cập nhật
        /// </summary>
        Update = 2,

        /// <summary>
        /// Cần xóa
        /// </summary>
        Delete = 3,
    }
}
