using Dapper;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using System.Linq.Expressions;
using System.Text;
using HoangCN.Core.Common.Metadata;
using HoangCN.Core.BL.Utils;

namespace HoangCN.Core.DL.Utils
{
    /// <summary>
    /// Lớp chứa các phương thức hỗ trợ tạo câu lệnh raw SQL sử dụng cache metadata để tối ưu hóa hiệu năng
    /// </summary>
    public static class BuildSQLUtil
    {
        /// <summary>
        /// Hàm hỗ trợ xây câu lệnh lấy các thông tin của dto theo điều kiện
        /// </summary>
        public static string BuildQueryStringGetDtoByCondition<TEntity, TResult>(
            bool isGetOnlyOne,
            Expression<Func<TEntity, bool>> condition, 
            DynamicParameters parameters) where TEntity : BaseEntity
        {
            var advancedFilters = ExpressionToFilterTranslator.Translate(condition);
            var sql = $"{BuildSelectClaude<TEntity, TResult>()}";
            var whereClause = BuildWhereClaude<TEntity, TResult>(new GetRequest(), parameters, advancedFilters);
            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += $" {whereClause}";
            }
            if (isGetOnlyOne)
            {
                sql += " LIMIT 1";
            }
            return sql;
        }

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
        /// Hàm hỗ trợ build điều kiện lọc theo từ khóa Key dựa trên các trường string của cả TEntity và TResult
        /// </summary>
        public static string BuildKeySearchClaude<TEntity, TResult>(string? key, DynamicParameters parameters) where TEntity : BaseEntity
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var resultMetadata = EntityMetadataCache.GetMetadata(typeof(TResult));

            var mainTableName = metadata.EntityType.Name;
            var mainColumns = metadata.ColumnNames;
            var resultProps = resultMetadata.Properties;

            var searchConditions = new List<string>();

            foreach (var prop in resultProps)
            {
                if (prop.IsNotMapped) continue;
                if (prop.PropertyInfo.PropertyType != typeof(string)) continue;

                var propName = prop.PropertyName;
                var foreignAttr = prop.ForeignTableAttr;

                if (foreignAttr != null)
                {
                    var foreignType = foreignAttr.EntityType;
                    if (foreignType != null)
                    {
                        var foreignTableName = foreignType.Name;
                        var columnNameInDb = !string.IsNullOrEmpty(foreignAttr.ColumnName) ? foreignAttr.ColumnName : propName;
                        
                        var foreignMetadata = EntityMetadataCache.GetMetadata(foreignType);
                        if (foreignMetadata.ColumnNames.Contains(columnNameInDb))
                        {
                            searchConditions.Add($"`{foreignTableName}`.`{columnNameInDb}` LIKE @SearchKey");
                        }
                    }
                }
                else
                {
                    if (mainColumns.Contains(propName))
                    {
                        searchConditions.Add($"`{mainTableName}`.`{propName}` LIKE @SearchKey");
                    }
                }
            }

            // Fallback: nếu DTO không có trường string nào được chọn, tự động quét các cột string của TEntity gốc
            if (searchConditions.Count == 0)
            {
                var stringProps = metadata.Properties
                    .Where(p => p.PropertyInfo.PropertyType == typeof(string) && !p.IsNotMapped)
                    .Select(p => p.PropertyName)
                    .ToList();
                foreach (var propName in stringProps)
                {
                    searchConditions.Add($"`{mainTableName}`.`{propName}` LIKE @SearchKey");
                }
            }

            if (searchConditions.Count > 0)
            {
                parameters.Add("SearchKey", $"%{key}%");
                var orConditions = string.Join(" OR ", searchConditions);
                return $"({orConditions})";
            }

            return string.Empty;
        }

        /// <summary>
        /// Giải quyết thuộc tính lọc thành tên cột SQL đầy đủ
        /// </summary>
        public static string ResolveSqlColumn<TEntity, TResult>(string propertyName, out string matchedPropertyName) where TEntity : BaseEntity
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var resultMetadata = EntityMetadataCache.GetMetadata(typeof(TResult));
            var mainTableName = metadata.EntityType.Name;
            var mainColumns = metadata.ColumnNames;
            var resultProps = resultMetadata.Properties;

            // 1. Tìm property tương ứng trong DTO (TResult)
            var matchedDtoProp = resultProps.FirstOrDefault(p => p.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (matchedDtoProp != null && !matchedDtoProp.IsNotMapped)
            {
                var foreignAttr = matchedDtoProp.ForeignTableAttr;
                if (foreignAttr != null)
                {
                    var foreignType = foreignAttr.EntityType;
                    if (foreignType != null)
                    {
                        var foreignTableName = foreignType.Name;
                        var columnNameInDb = !string.IsNullOrEmpty(foreignAttr.ColumnName) ? foreignAttr.ColumnName : matchedDtoProp.PropertyName;
                        
                        var foreignMetadata = EntityMetadataCache.GetMetadata(foreignType);
                        if (foreignMetadata.ColumnNames.Contains(columnNameInDb))
                        {
                            matchedPropertyName = matchedDtoProp.PropertyName;
                            return $"`{foreignTableName}`.`{columnNameInDb}`";
                        }
                    }
                }
                else
                {
                    var propName = matchedDtoProp.PropertyName;
                    if (mainColumns.Contains(propName))
                    {
                        matchedPropertyName = propName;
                        return $"`{mainTableName}`.`{propName}`";
                    }
                }
            }

            // 2. Fallback: Nếu không tìm thấy trên DTO hoặc không map, tìm trên thực thể chính TEntity
            var matchedEntityProp = mainColumns.FirstOrDefault(c => c.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (matchedEntityProp != null)
            {
                matchedPropertyName = matchedEntityProp;
                return $"`{mainTableName}`.`{matchedEntityProp}`";
            }

            throw new BadRequestException($"Thuộc tính lọc '{propertyName}' không tồn tại hoặc không được ánh xạ");
        }

        /// <summary>
        /// Hàm hỗ trợ build toàn bộ mệnh đề WHERE (bao gồm Ids, Filters, từ khóa Key và các nhóm bộ lọc nâng cao)
        /// </summary>
        public static string BuildWhereClaude<TEntity, TResult>(
            GetRequest request, 
            DynamicParameters parameters, 
            AdvancedFilterGroup? extraGroup = null) where TEntity : BaseEntity
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var mainTableName = metadata.EntityType.Name;
            var whereConditions = new List<string>();

            // 0. Lọc bỏ dữ liệu đã bị xóa mềm mặc định
            whereConditions.Add($"`{mainTableName}`.`IsDeleted` = 0");

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
                var filterWhere = BuildWhereClaudeFromFilters<TEntity, TResult>(request.Filters, request.FilterGroupType, parameters);
                if (!string.IsNullOrEmpty(filterWhere) && filterWhere != "TRUE")
                {
                    whereConditions.Add($"({filterWhere})");
                }
            }

            // 3. Lọc theo từ khóa tìm kiếm (Key)
            var keySearch = BuildKeySearchClaude<TEntity, TResult>(request.Key, parameters);
            if (!string.IsNullOrEmpty(keySearch))
            {
                whereConditions.Add(keySearch);
            }

            // 4. Lọc theo AdvancedFilterGroup từ client và từ server
            int paramCounter = 0;
            if (request.AdvancedFilterGroup != null)
            {
                var groupSql = BuildWhereClauseFromGroup<TEntity, TResult>(request.AdvancedFilterGroup, parameters, ref paramCounter);
                if (!string.IsNullOrEmpty(groupSql))
                {
                    whereConditions.Add($"({groupSql})");
                }
            }

            if (extraGroup != null)
            {
                var extraSql = BuildWhereClauseFromGroup<TEntity, TResult>(extraGroup, parameters, ref paramCounter);
                if (!string.IsNullOrEmpty(extraSql))
                {
                    whereConditions.Add($"({extraSql})");
                }
            }

            // Hợp nhất các điều kiện WHERE
            return whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions) 
                : string.Empty;
        }

        /// <summary>
        /// Hàm xây dựng SQL đệ quy cho AdvancedFilterGroup
        /// </summary>
        public static string BuildWhereClauseFromGroup<TEntity, TResult>(
            AdvancedFilterGroup group, 
            DynamicParameters parameters, 
            ref int paramCounter) where TEntity : BaseEntity
        {
            if (group == null) return string.Empty;

            var conditions = new List<string>();

            // 1. Lọc theo các bộ lọc Filters cấp hiện tại
            if (group.Filters != null && group.Filters.Count > 0)
            {
                foreach (var filter in group.Filters)
                {
                    var sqlColumn = ResolveSqlColumn<TEntity, TResult>(filter.Property, out var matchedPropName);
                    filter.Property = matchedPropName; // Đồng bộ hoa thường

                    if (!filter.Operator.IsValidOperator(filter.Type))
                    {
                        throw new BadRequestException("Toán tử không hợp lệ cho kiểu dữ liệu lọc");
                    }

                    if (!string.IsNullOrEmpty(filter.ColumnToCompare))
                    {
                        // So sánh cột với cột
                        var sqlColumn2 = ResolveSqlColumn<TEntity, TResult>(filter.ColumnToCompare, out var matchedPropName2);
                        filter.ColumnToCompare = matchedPropName2;

                        var condition = filter.Operator.ToSQLKeyword(sqlColumn, sqlColumn2);
                        // Bỏ ký tự '@' vì vế phải là tên cột chứ không phải tham số
                        if (condition.Contains($"@{sqlColumn2}"))
                        {
                            condition = condition.Replace($"@{sqlColumn2}", sqlColumn2);
                        }
                        conditions.Add(condition);
                    }
                    else
                    {
                        if (filter.Value == null)
                        {
                            var condition = filter.Operator switch
                            {
                                FilterOperator.Equal => $"{sqlColumn} IS NULL",
                                FilterOperator.NotEqual => $"{sqlColumn} IS NOT NULL",
                                _ => throw new BadRequestException($"Toán tử '{filter.Operator}' không hợp lệ cho giá trị NULL.")
                            };
                            conditions.Add(condition);
                        }
                        else
                        {
                            // So sánh cột với giá trị
                            var paramName = $"af_{matchedPropName}_{paramCounter++}";
                            var condition = filter.Operator.ToSQLKeyword(sqlColumn, paramName);

                            if (filter.Operator == FilterOperator.In || filter.Operator == FilterOperator.NotIn)
                            {
                                parameters.Add(paramName, filter.Value);
                            }
                            else if (filter.Type == FilterType.Number)
                            {
                                var rawValue = GetValueFromFilter(filter.Value);
                                parameters.Add(paramName, Convert.ChangeType(rawValue!, typeof(int)));
                            }
                            else if (filter.Type == FilterType.Bool)
                            {
                                var rawValue = GetValueFromFilter(filter.Value);
                                parameters.Add(paramName, Convert.ToBoolean(rawValue!));
                            }
                            else if (filter.Type == FilterType.Date)
                            {
                                var rawValue = GetValueFromFilter(filter.Value);
                                parameters.Add(paramName, rawValue is DateTime dt ? dt : DateTime.Parse(rawValue!.ToString()!));
                            }
                            else if (filter.Type == FilterType.String)
                            {
                                var rawValue = GetValueFromFilter(filter.Value);
                                parameters.Add(paramName, rawValue?.ToString() ?? string.Empty);
                            }
                            else
                            {
                                throw new BadRequestException("Kiểu dữ liệu lọc không được hỗ trợ");
                            }

                            conditions.Add(condition);
                        }
                    }
                }
            }

            // 2. Duyệt đệ quy qua các nhóm con Groups
            if (group.Groups != null && group.Groups.Count > 0)
            {
                foreach (var subGroup in group.Groups)
                {
                    var subSql = BuildWhereClauseFromGroup<TEntity, TResult>(subGroup, parameters, ref paramCounter);
                    if (!string.IsNullOrEmpty(subSql))
                    {
                        conditions.Add($"({subSql})");
                    }
                }
            }

            if (conditions.Count == 0) return string.Empty;

            var opJoin = group.GroupType == FilterGroupType.And ? " AND " : " OR ";
            return string.Join(opJoin, conditions);
        }

        /// <summary>
        /// Hàm hỗ trợ xây SQL phần điều kiện câu lệnh WHERE từ danh sách Filter hỗ trợ DTO
        /// </summary>
        public static string BuildWhereClaudeFromFilters<TEntity, TResult>(List<Filter>? filters, FilterGroupType groupType, DynamicParameters parameters) where TEntity : BaseEntity
        {
            if (filters == null || filters.Count == 0) return "TRUE";

            var conditions = new List<string>();

            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];
                var sqlColumn = ResolveSqlColumn<TEntity, TResult>(filter.Property, out var matchedPropName);
                filter.Property = matchedPropName; // Đồng bộ lại tên hoa thường chuẩn

                if (!filter.Operator.IsValidOperator(filter.Type))
                {
                    throw new BadRequestException("Toán tử không hợp lệ");
                }

                if (!string.IsNullOrEmpty(filter.ColumnToCompare))
                {
                    var sqlColumn2 = ResolveSqlColumn<TEntity, TResult>(filter.ColumnToCompare, out var matchedPropName2);
                    filter.ColumnToCompare = matchedPropName2;

                    var condition = filter.Operator.ToSQLKeyword(sqlColumn, sqlColumn2);
                    if (condition.Contains($"@{sqlColumn2}"))
                    {
                        condition = condition.Replace($"@{sqlColumn2}", sqlColumn2);
                    }
                    conditions.Add(condition);
                }
                else
                {
                    if (filter.Value == null)
                    {
                        var condition = filter.Operator switch
                        {
                            FilterOperator.Equal => $"{sqlColumn} IS NULL",
                            FilterOperator.NotEqual => $"{sqlColumn} IS NOT NULL",
                            _ => throw new BadRequestException($"Toán tử '{filter.Operator}' không hợp lệ cho giá trị NULL.")
                        };
                        conditions.Add(condition);
                    }
                    else
                    {
                        var paramName = $"{filter.Property}_{i}";
                        var condition = filter.Operator.ToSQLKeyword(sqlColumn, paramName);

                        if (filter.Operator == FilterOperator.In || filter.Operator == FilterOperator.NotIn)
                        {
                            parameters.Add(paramName, filter.Value);
                        }
                        else if (filter.Type == FilterType.Number)
                        {
                            var rawValue = GetValueFromFilter(filter.Value);
                            parameters.Add(paramName, Convert.ChangeType(rawValue!, typeof(int)));
                        }
                        else if (filter.Type == FilterType.Bool)
                        {
                            var rawValue = GetValueFromFilter(filter.Value);
                            parameters.Add(paramName, Convert.ToBoolean(rawValue!));
                        }
                        else if (filter.Type == FilterType.Date)
                        {
                            var rawValue = GetValueFromFilter(filter.Value);
                            parameters.Add(paramName, rawValue is DateTime dt ? dt : DateTime.Parse(rawValue!.ToString()!));
                        }
                        else if (filter.Type == FilterType.String)
                        {
                            var rawValue = GetValueFromFilter(filter.Value);
                            parameters.Add(paramName, rawValue?.ToString() ?? string.Empty);
                        }
                        else
                        {
                            throw new InvalidDataException("Kiểu lọc chưa hỗ trợ");
                        }

                        conditions.Add(condition);
                    }
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

        public static string BuildFromClaude<TEntity, TResult>()
        {
            var mainTableName = typeof(TEntity).Name;
            var mainMetadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var mainColumns = mainMetadata.ColumnNames;

            var resultType = typeof(TResult);
            var resultMetadata = EntityMetadataCache.GetMetadata(resultType);
            var resultProps = resultMetadata.Properties;

            var joinClauses = new List<string>();
            var joinedTables = new HashSet<Type>();

            foreach (var prop in resultProps)
            {
                if (prop.IsNotMapped) continue;

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
                            if (!joinedTables.Contains(foreignType))
                            {
                                joinedTables.Add(foreignType);
                                var foreignKeyId = $"{foreignTableName}Id";
                                joinClauses.Add($"LEFT JOIN `{foreignTableName}` ON `{mainTableName}`.`{foreignKeyId}` = `{foreignTableName}`.`{foreignKeyId}`");
                            }
                        }
                    }
                }
            }

            var joinsSql = joinClauses.Count > 0 ? " " + string.Join(" ", joinClauses) : string.Empty;
            return $"FROM `{mainTableName}`{joinsSql}";
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

            foreach (var prop in resultProps)
            {
                if (prop.IsNotMapped) continue;

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
            return $"SELECT {columnsSql} {BuildFromClaude<TEntity, TResult>()}";
        }

        
        private static object? GetValueFromFilter(object? value)
        {
            if (value == null) return null;

            if (value is System.Text.Json.JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case System.Text.Json.JsonValueKind.Number:
                        if (jsonElement.TryGetInt32(out int intVal)) return intVal;
                        if (jsonElement.TryGetInt64(out long longVal)) return longVal;
                        if (jsonElement.TryGetDouble(out double doubleVal)) return doubleVal;
                        return jsonElement.GetDecimal();
                    case System.Text.Json.JsonValueKind.String:
                        return jsonElement.GetString();
                    case System.Text.Json.JsonValueKind.True:
                        return true;
                    case System.Text.Json.JsonValueKind.False:
                        return false;
                    case System.Text.Json.JsonValueKind.Null:
                        return null;
                    default:
                        return jsonElement.GetRawText();
                }
            }

            var type = value.GetType();
            if (type.IsEnum)
            {
                return Convert.ChangeType(value, Enum.GetUnderlyingType(type));
            }

            return value;
        }
    }
}

