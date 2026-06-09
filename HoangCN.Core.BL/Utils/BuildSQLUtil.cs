using Dapper;
using HoangCN.Common.Base;
using HoangCN.Common.Enums;
using HoangCN.Common.Exceptions;
using HoangCN.Common.Model.Requests;
using HoangCN.Common.Utils;
using HoangCN.Core.BL.Metadata;
using System.Linq.Expressions;
using System.Text;

namespace HoangCN.Core.BL.Utils
{
    /// <summary>
    /// Lớp chứa các phương thức hỗ trợ tạo câu lệnh raw SQL sử dụng cache metadata để tối ưu hóa hiệu năng
    /// </summary>
    public static class BuildSQLUtil
    {
        /// <summary>
        /// Tạo câu lệnh kiểm tra sự tồn tại của danh sách các ID khóa chính
        /// </summary>
        public static string BuildQueryStringCheckPkExists(string tableName, string pkPropName)
        {
            return $"SELECT `{pkPropName}` FROM `{tableName}` WHERE `{pkPropName}` IN @Ids";
        }

        /// <summary>
        /// Tạo câu lệnh kiểm tra trùng lặp (lấy cả ID và cột trùng để lọc loại trừ ID hiện tại)
        /// </summary>
        public static string BuildQueryStringCheckDuplicate(string tableName, string pkPropName, string columnName)
        {
            return $"SELECT `{pkPropName}`, `{columnName}` FROM `{tableName}` WHERE `{columnName}` IN @Values";
        }

        /// <summary>
        /// Tạo câu lệnh kiểm tra sự tồn tại của khóa ngoại trên bảng đích
        /// </summary>
        public static string BuildQueryStringCheckFkExists(string tableName, string columnName)
        {
            return $"SELECT `{columnName}` FROM `{tableName}` WHERE `{columnName}` IN @Values";
        }

        /// <summary>
        /// Hàm hỗ trợ build phần phân trang (LIMIT ... OFFSET ...)
        /// </summary>
        public static string BuildPagingClaude(int page, int size, DynamicParameters parameters)
        {
            parameters.Add("Limit", size);
            parameters.Add("Offset", (page - 1) * size);
            return " LIMIT @Limit OFFSET @Offset";
        }

        /// <summary>
        /// Hàm hỗ trợ build điều kiện lọc theo từ khóa Key
        /// </summary>
        public static string BuildKeySearchClaude<TEntity>(string? key, DynamicParameters parameters) where TEntity : BaseEntity
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var mainTableName = metadata.EntityType.Name;

            // Lấy danh sách các trường chuỗi thực tế từ cache
            var stringProps = metadata.Properties
                .Where(p => p.PropertyInfo.PropertyType == typeof(string) && !p.IsNotMapped)
                .Select(p => p.PropertyName)
                .ToList();

            if (stringProps.Count > 0)
            {
                var orConditions = string.Join(" OR ", stringProps.Select(propName => $"`{mainTableName}`.`{propName}` LIKE @SearchKey"));
                parameters.Add("SearchKey", $"%{key}%");
                return $"({orConditions})";
            }

            return string.Empty;
        }

        /// <summary>
        /// Hàm hỗ trợ build toàn bộ mệnh đề WHERE (bao gồm Ids, Filters, và từ khóa Key)
        /// </summary>
        public static string BuildWhereClaude<TEntity>(GetRequest request, DynamicParameters parameters) where TEntity : BaseEntity
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var mainTableName = metadata.EntityType.Name;
            var whereConditions = new List<string>();

            // 1. Lọc theo danh sách Ids (nếu có)
            if (request.Ids != null && request.Ids.Count > 0)
            {
                var pkPropName = metadata.PrimaryKeyName;
                whereConditions.Add($"`{mainTableName}`.`{pkPropName}` IN @Ids");
                parameters.Add("Ids", request.Ids);
            }

            // 2. Lọc theo bộ lọc động request.Filters
            if (request.Filters != null && request.Filters.Count > 0)
            {
                var filterWhere = BuildWhereClaudeFromFilters<TEntity>(request.Filters, request.FilterGroupType, parameters);
                if (!string.IsNullOrEmpty(filterWhere) && filterWhere != "TRUE")
                {
                    whereConditions.Add(filterWhere);
                }
            }

            // 3. Lọc theo từ khóa tìm kiếm (Key)
            var keySearch = BuildKeySearchClaude<TEntity>(request.Key, parameters);
            if (!string.IsNullOrEmpty(keySearch))
            {
                whereConditions.Add(keySearch);
            }

            // Hợp nhất các điều kiện WHERE
            return whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions) 
                : string.Empty;
        }

        /// <summary>
        /// Hàm hỗ trợ xây SQL phần điều kiện câu lệnh WHERE từ danh sách Filter
        /// </summary>
        public static string BuildWhereClaudeFromFilters<TEntity>(List<Filter>? filters, FilterGroupType groupType, DynamicParameters parameters)
        {
            if (filters == null || filters.Count == 0) return "TRUE";

            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var columnNames = metadata.ColumnNames;
            var conditions = new List<string>();

            foreach (var filter in filters)
            {
                // Tìm property khớp không phân biệt hoa thường từ Cache
                var matchedProp = columnNames.FirstOrDefault(c => c.Equals(filter.Property, StringComparison.OrdinalIgnoreCase));
                if (matchedProp == null)
                {
                    throw new BadRequestException("Thuộc tính lọc không tồn tại");
                }
                
                filter.Property = matchedProp;

                if (!filter.Operator.IsValidOperator(filter.Type))
                {
                    throw new BadRequestException("Toán tử không hợp lệ");
                }

                var condition = filter.Operator.ToSQLKeyword($"`{metadata.EntityType.Name}`.`{filter.Property}`", $"{filter.Property}");

                // Gán tham số dựa trên kiểu dữ liệu lọc
                if (filter.Type == FilterType.Number)
                {
                    parameters.Add($"@{filter.Property}", int.Parse(filter.Value.ToString()!));
                }
                else if (filter.Type == FilterType.String)
                {
                    parameters.Add($"@{filter.Property}", filter.Value.ToString()!);
                }
                else if (filter.Type == FilterType.Bool)
                {
                    parameters.Add($"@{filter.Property}", bool.Parse(filter.Value.ToString()!));
                }
                else
                {
                    throw new InvalidDataException("Kiểu lọc chưa hỗ trợ");
                }

                if (condition != null)
                {
                    conditions.Add(condition);
                }
            }

            if (groupType == FilterGroupType.And)
            {
                return string.Join(" AND ", conditions);
            }

            return string.Join(" OR ", conditions);
        }

        /// <summary>
        /// Hàm hỗ trợ build phần sắp xếp ORDER BY
        /// </summary>
        public static string BuildSortClaude<TEntity>(GetRequest? pagingRequest = null)
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var mainTableName = metadata.EntityType.Name;

            // Ưu tiên chọn tiêu chí sắp xếp được gửi lên
            if (pagingRequest != null && 
                !string.IsNullOrEmpty(pagingRequest.SortBy) &&
                pagingRequest.IsAsc != null)
            {
                if (metadata.ColumnNames.Contains(pagingRequest.SortBy))
                {
                    return $"ORDER BY `{mainTableName}`.`{pagingRequest.SortBy}` {(pagingRequest.IsAsc.Value ? "ASC" : "DESC")}";
                }
            }

            // Nếu ko gửi lên thì chọn mặc định đã được cấu hình trong SearchConfigAttribute từ cache
            var attr = metadata.SearchConfigAttr;
            if (attr != null)
            {
                return $"ORDER BY `{mainTableName}`.`{attr.DefaultSortBy}` {(attr.DefaultIsAsc ? "ASC" : "DESC")}";
            }

            // Nếu chưa cấu hình thì sort theo ModifiedDate giảm dần
            return $"ORDER BY `{mainTableName}`.`ModifiedDate` DESC";
        }

        /// <summary>
        /// Hàm hỗ trợ build phần chọn trường trả về SELECT ... FROM ... JOIN
        /// </summary>
        public static string BuildSelectClaude<TEntity, TResult>() where TEntity : BaseEntity
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var resultMetadata = EntityMetadataCache.GetMetadata(typeof(TResult));

            var mainTableName = metadata.EntityType.Name;
            var mainColumns = metadata.ColumnNames;
            var resultProps = resultMetadata.Properties;

            var selectColumns = new List<string>();
            var joinClauses = new List<string>();
            var joinedTables = new HashSet<Type>();

            foreach (var prop in resultProps)
            {
                if (prop.IsNotMapped) continue;

                // Kiểm tra xem thuộc tính có ForeignTableAttribute hay không
                var foreignAttr = prop.ForeignTableAttr;
                if (foreignAttr != null)
                {
                    var foreignType = foreignAttr.EntityType;
                    if (foreignType != null)
                    {
                        var foreignTableName = foreignType.Name;
                        var propName = prop.PropertyName;
                        var columnNameInDb = !string.IsNullOrEmpty(foreignAttr.ColumnName) ? foreignAttr.ColumnName : propName;

                        var foreignMetadata = EntityMetadataCache.GetMetadata(foreignType);
                        var foreignColumns = foreignMetadata.ColumnNames;
                        
                        if (foreignColumns.Contains(columnNameInDb))
                        {
                            selectColumns.Add($"`{foreignTableName}`.`{columnNameInDb}` AS `{propName}`");

                            // Thêm LEFT JOIN
                            if (!joinedTables.Contains(foreignType))
                            {
                                joinedTables.Add(foreignType);
                                var foreignKeyId = $"{foreignTableName}Id";
                                joinClauses.Add($"LEFT JOIN `{foreignTableName}` ON `{mainTableName}`.`{foreignKeyId}` = `{foreignTableName}`.`{foreignKeyId}`");
                            }
                        }
                    }
                }
                else
                {
                    var propName = prop.PropertyName;
                    if (mainColumns.Contains(propName))
                    {
                        selectColumns.Add($"`{mainTableName}`.`{propName}` AS `{propName}`");
                    }
                }
            }

            var columnsSql = string.Join(", ", selectColumns);
            var joinsSql = joinClauses.Count > 0 ? " " + string.Join(" ", joinClauses) : string.Empty;

            return $"SELECT {columnsSql} FROM `{mainTableName}`{joinsSql}";
        }

        /// <summary>
        /// Tạo mệnh đề WHERE từ Expression condition
        /// </summary>
        public static string BuildWhereClauseFromExpression<TEntity>(Expression<Func<TEntity, bool>> condition, out DynamicParameters parameters) where TEntity : BaseEntity
        {
            if (condition == null)
            {
                parameters = new DynamicParameters();
                return string.Empty;
            }

            var (sql, paramsOut) = ExpressionToSqlTranslator.Translate(condition);
            parameters = paramsOut;
            return string.IsNullOrEmpty(sql) ? string.Empty : $"WHERE {sql}";
        }
    }
}
