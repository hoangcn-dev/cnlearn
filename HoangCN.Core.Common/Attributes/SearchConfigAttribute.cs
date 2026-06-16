namespace HoangCN.Core.Common.Attributes
{
    /// <summary>
    /// Thuộc tính cấu hình cho tìm kiếm thực thế
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SearchConfigAttribute : Attribute
    {
        /// <summary>
        /// Thuộc tính dùng để sắp xếp mặc định
        /// </summary>
        public string DefaultSortBy { get; set; }

        /// <summary>
        /// Sắp xếp tăng dần hay giảm dần
        /// </summary>
        public bool DefaultIsAsc { get; set; }
    }
}

