namespace HoangCN.Common.Attributes
{
    /// <summary>
    /// Đánh dấu thuộc tính này là đến từ bảng ngoại
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignTableAttribute : Attribute
    {
        /// <summary>
        /// Entity đại diệu cho bảng ngoại
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Tên cột ở bảng ngoại (sử dụng trong trường hợp tên cột ở DB khác với tên trường ở Dto)
        /// </summary>
        public string? ColumnName { get; set; }
    }
}
