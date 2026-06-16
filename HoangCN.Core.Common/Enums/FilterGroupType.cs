using System.Text.Json.Serialization;

namespace HoangCN.Core.Common.Enums
{
    /// <summary>
    /// Loại gộp các filters
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
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

