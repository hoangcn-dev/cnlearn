using HoangCN.Core.Common.Enums;
using System.Text.Json.Serialization;

namespace HoangCN.Core.Common.Model.Requests
{
    /// <summary>
    /// Request tìm kiếm
    /// </summary>
    public class GetRequest
    {
        /// <summary>
        /// Số trang
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Kích thước trang
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Trường cần sort
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Tăng hoặc giảm dần
        /// </summary>
        public bool? IsAsc { get; set; }

        /// <summary>
        /// Có phân trang hay không
        /// </summary>
        public bool IsPaging { get; set; }

        /// <summary>
        /// Từ khóa
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Danh sách Ids cần lấy
        /// </summary>
        public List<Guid> Ids { get; set; } = [];

        /// <summary>
        /// Bộ lọc
        /// </summary>
        public List<Filter> Filters { get; set; } = [];

        /// <summary>
        /// Kiểm gộp điều kiện: Or / And
        /// </summary>
        public FilterGroupType FilterGroupType { get; set; } = FilterGroupType.Or;

        /// <summary>
        /// Factory cho tham số Get All
        /// </summary>
        public static GetRequest GetAllRequest()
        {
            return new GetRequest();
        }

        /// <summary>
        /// Factory cho tham số Get By Id
        /// </summary>
        public static GetRequest GetByIdRequest(Guid Id)
        {
            return new GetRequest
            {
                Ids = [Id],
            };
        }
    }

    /// <summary>
    /// Bộ lọc
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Tên thuộc tính cần lọc
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Toán tử lọc
        /// </summary>
        public FilterOperator Operator { get; set; }

        /// <summary>
        /// Giá trị lọc
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Kiểu giá trị của cột cần lọc
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FilterType Type { get; set; }
    }
}

