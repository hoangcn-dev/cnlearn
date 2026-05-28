namespace HoangCN.Common.Enums
{
    /// <summary>
    /// Loại gộp các filters
    /// </summary>
    public enum FilterGroupType
    {
        /// <summary>
        /// Tất cả filter phải thỏa mãn
        /// </summary>
        And = 0,

        /// <summary>
        /// Ít nhất 1 filter thỏa mãn 
        /// </summary>
        Or = 1,
    }
}
