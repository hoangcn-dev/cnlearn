namespace HoangCN.Core.Common.Enums
{
    /// <summary>
    /// Toán tử để lọc
    /// </summary>
    public enum FilterOperator
    {
        /// <summary>
        /// Dấu =
        /// </summary>
        Equal,

        /// <summary>
        /// Dấu !=
        /// </summary>
        NotEqual,

        /// <summary>
        /// Dấu <
        /// </summary>
        LessThan,

        /// <summary>
        /// Dấu <=
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Dấu >
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Dấu >=
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Dấu *
        /// </summary>
        Contain,

        /// <summary>
        /// Dấu !*
        /// </summary>
        NotContain,

        /// <summary>
        /// Dấu +
        /// </summary>
        StartWith,

        /// <summary>
        /// Dấu -
        /// </summary>
        EndWith,
    }

    /// <summary>
    /// Lớp tiện ích cho toán tử lọc
    /// </summary>
    public static class FilterOperatorExtension
    {
        /// <summary>
        /// Hàm tạo một mệnh đề so sánh từ toán tử, thuộc tính và tên tham số
        /// </summary>
        public static string ToSQLKeyword(this FilterOperator o, string propertyName, string paramName)
        {
            return o switch
            {
                FilterOperator.Equal => $"{propertyName} = @{paramName}",

                FilterOperator.NotEqual => $"{propertyName} != @{paramName}",

                FilterOperator.LessThan => $"{propertyName} < @{paramName}",

                FilterOperator.LessThanOrEqual => $"{propertyName} <= @{paramName}",

                FilterOperator.GreaterThan => $"{propertyName} > @{paramName}",

                FilterOperator.GreaterThanOrEqual => $"{propertyName} >= @{paramName}",

                FilterOperator.Contain => $"{propertyName} LIKE CONCAT('%', @{paramName}, '%')",

                FilterOperator.NotContain => $"{propertyName} NOT LIKE CONCAT('%', @{paramName}, '%')",

                FilterOperator.StartWith => $"{propertyName} LIKE CONCAT(@{paramName}, '%')",

                FilterOperator.EndWith => $"{propertyName} LIKE CONCAT('%', @{paramName})",

                _ => string.Empty
            };
        }

        /// <summary>
        /// Kiểm tra xem toán tử lọc có phù hợp với kiểu giá trị hay không
        /// </summary>
        public static bool IsValidOperator(this FilterOperator o, FilterType filterType)
        {
            if (filterType == FilterType.Number)
            {
                return 
                    o == FilterOperator.Equal || 
                    o == FilterOperator.NotEqual || 
                    o == FilterOperator.LessThan ||
                    o == FilterOperator.LessThanOrEqual ||
                    o == FilterOperator.GreaterThan ||
                    o == FilterOperator.GreaterThanOrEqual;
            }
            else if (filterType == FilterType.String)
            {
                return
                    o == FilterOperator.Equal ||
                    o == FilterOperator.NotEqual ||
                    o == FilterOperator.StartWith ||
                    o == FilterOperator.EndWith ||
                    o == FilterOperator.Contain ||
                    o == FilterOperator.NotContain;
            }
            else if (filterType == FilterType.Bool)
            {
                return
                    o == FilterOperator.Equal;
            }

            return false;
        }
    }
}

