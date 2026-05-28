using Dapper;
using HoangCN.Common.Enums;
using HoangCN.Common.Exceptions;
using HoangCN.Common.Attributes;
using HoangCN.Common.Model.Requests;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using HoangCN.Common.Base;
using HoangCN.Common.Utils;
using System.Linq.Expressions;

namespace HoangCN.BL.Utils
{
    /// <summary>
    /// Lớp chứa các phương thức hỗ trợ tạo câu lệnh raw SQL cho 1 entity
    /// </summary>
    public class BuildSQLUtil
    {

        /// <summary>
        /// Tạo câu lệnh thêm mới / cập nhật danh sách các entities
        /// </summary>
        public static string BuildQueryStringInsertOrUpdate<TEntity>(List<TEntity> entities, out DynamicParameters parameters) where TEntity : BaseEntity
        {
            parameters = new DynamicParameters();

            // Kiểm tra null và rỗng
            if (entities == null || entities.Count == 0)
            {
                return string.Empty;
            }

            var sql = new StringBuilder();
            var entityType = entities[0].GetType();
            var tableName = entityType.Name;
            var pkPropName = $"{entityType.Name}Id";
            var columnNames = entityType.GetAllColumnProps();

            /* Câu lệnh mẫu
            INSERT INTO Employee (EmployeeId, EmployeeCode, EmployeeName, EmployeeEmail)
            VALUES 
                (@EmployeeId1, @EmployeeCode1, @EmployeeName1, @EmployeeEmail1),
                (@EmployeeId2, @EmployeeCode2, @EmployeeName2, @EmployeeEmail2),
                ...
                (@EmployeeIdn, @EmployeeCoden, @EmployeeNamen, @EmployeeEmailn)
            ON DUPLICATE KEY UPDATE
                EmployeeCode = VALUES(EmployeeCode),
                EmployeeName = VALUES(EmployeeName),
                EmployeeEmail = VALUES(EmployeeEmail);
            */

            // Bước 1: Thêm phần liệt kê các cột chung cho bảng của TEntity
            sql.AppendLine($"INSERT INTO `{tableName}` ({string.Join(", ", columnNames.Select(col => $"`{col}`"))}) ");

            // Bước 2: Thêm phần liệt kê tham số cho tạo mới
            int postfix = 0;
            var createLines = new List<string>();
            sql.AppendLine($"VALUES ");
            foreach (var e in entities)
            {
                postfix++;
                var param = $"({string.Join(", ", columnNames.Select(col => "@" + col + postfix))})";
                createLines.Add(param);
            }
            sql.AppendLine(string.Join(",\n", createLines));

            // Bước 3: Thêm phần liệt kê tham số cho cập nhật
            var skipWhenUpdateColumns = new HashSet<string>
            {
                pkPropName,
                nameof(BaseEntity.CreatedBy),
                nameof(BaseEntity.CreatedDate),
            };
            sql.AppendLine("ON DUPLICATE KEY UPDATE ");
            sql.AppendLine(string.Join(",\n", columnNames
                    .Where(col => !skipWhenUpdateColumns.Contains(col))
                    .Select(col => $"`{col}` = VALUES(`{col}`)")));
            sql.Append(';');

            // Bước 4: Thêm các param từ danh sách entities
            postfix = 0;
            foreach (var e in entities)
            {
                postfix++;
                foreach (var colName in columnNames)
                {
                    var colValue = e.GetValueByPropName(colName);

                    if (colValue is null or string or int or Guid or DateTime or decimal or double or bool or Enum)
                    {
                        parameters.Add(colName + postfix, colValue);
                    }
                    else
                    {
                        throw new NotImplementedException($"Chưa triển khai xử lý đối với prop kiểu {colValue.GetType().Name}");
                    }
                }
            }

            return sql.ToString();
        }


        /// <summary>
        /// Tạo câu lệnh xóa các entities
        /// </summary>
        public static string BuildQueryStringDelete<TEntity>(List<TEntity> entities, out DynamicParameters parameters) where TEntity : BaseEntity
        {
            parameters = new DynamicParameters();

            // Kiểm tra null và rỗng
            if (entities == null || entities.Count == 0)
            {
                return string.Empty;
            }

            var entityType = entities[0].GetType();
            var tableName = entityType.Name;
            var pkPropName = $"{entityType.Name}Id";

            var sql = $"DELETE FROM `{tableName}` WHERE `{pkPropName}` IN @Ids;";
            parameters.Add("Ids", entities.Select(e => e.GetValueByPropName(pkPropName)));

            return sql;
        }


        /// <summary>
        /// Tạo câu lệnh lấy tất cả entity
        /// </summary>
        public static string BuildQueryStringGetAll<TEntity>(GetRequest? request = null) where TEntity : BaseEntity
        {
            // Tạo câu lệnh SQL
            var tableName = typeof(TEntity).Name;
            var sort = BuildSortClaude<TEntity>(request);
            var sql = $"SELECT * FROM {tableName} {sort}";
            return sql;
        }


        /// <summary>
        /// Tạo câu lệnh lấy tất cả entity có ID nằm trong danh sách
        /// </summary>
        public static string BuildQueryStringGetByIds<TEntity>(GetRequest request, out DynamicParameters parameters) where TEntity : BaseEntity
        {
            parameters = new DynamicParameters();

            // Kiểm tra null và rỗng
            if (request.Ids == null || request.Ids.Count == 0)
            {
                return string.Empty;
            }

            var entityType = typeof(TEntity);
            var tableName = entityType.Name;
            var pkPropName = $"{entityType.Name}Id";

            // Tạo câu lệnh
            var sort = BuildSortClaude<TEntity>(request);
            var sql = $"SELECT * FROM {tableName} WHERE {pkPropName} IN @Ids {sort}";

            // Thêm tham số
            parameters.Add("Ids", request.Ids);

            return sql;
        }


        /// <summary>
        /// Hàm tạo câu lệnh tìm kiếm dựa trên bộ lọc và phân trang
        /// </summary>
        public static (string PagingQuery, string CountQuery) BuildQueryStringGetPaging<TEntity>(GetRequest request, out DynamicParameters parameters) where TEntity : BaseEntity
        {
            parameters = new DynamicParameters();

            var entityType = typeof(TEntity);
            var tableName = entityType.Name;
            var pkPropName = $"{entityType.Name}Id";

            // Câu lệnh phân trang & đếm
            var pagingSql = new StringBuilder($"SELECT * FROM {tableName}");
            var countSql = new StringBuilder($"SELECT COUNT(*) FROM {tableName}");

            // Thêm mệnh đề lọc
            var whereClaude = BuildWhereClaudeFromFilters<TEntity>(request.Filters, request.FilterGroupType, parameters);

            // 💡 Thêm điều kiện lọc từ khóa tìm kiếm 
            if (!string.IsNullOrEmpty(request.Key))
            {
                var stringProps = entityType.GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && !Attribute.IsDefined(p, typeof(NotMappedAttribute)))
                    .Select(p => p.Name)
                    .ToList();

                if (stringProps.Count > 0)
                {
                    var orConditions = string.Join(" OR ", stringProps.Select(propName => $"{propName} LIKE @SearchKey"));
                    if (whereClaude == "TRUE" || string.IsNullOrEmpty(whereClaude))
                    {
                        whereClaude = $"({orConditions})";
                    }
                    else
                    {
                        whereClaude = $"({whereClaude}) AND ({orConditions})";
                    }
                    parameters.Add("SearchKey", $"%{request.Key}%");
                }
            }

            pagingSql.Append($" \n WHERE {whereClaude}");
            countSql.Append($" \n WHERE {whereClaude}");

            // Thêm sắp xếp
            var sortClaude = BuildSortClaude<TEntity>(request);
            pagingSql.Append($"\n {sortClaude}");

            // Thêm phần phân trang
            if (request.Size.HasValue)
            {
                pagingSql.Append(" LIMIT @Limit");
                parameters.Add("Limit", request.Size);

                if (request.Page.HasValue)
                {
                    var offset = (request.Page - 1) * request.Size;
                    pagingSql.Append(" OFFSET @Offset");
                    parameters.Add("Offset", offset);
                }
            }

            return (pagingSql.ToString(), countSql.ToString());
        }


        /// <summary>
        /// Hàm tạo câu lệnh đếm số hàng bị trùng (phiên bản không định kiểu dùng cho khóa ngoại)
        /// </summary>
        public static string BuildQueryStringCountDuplicate(Type entityType, Guid? ignoreId, string columnName, object value, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();

            var tableName = entityType.Name;
            var pkPropName = $"{entityType.Name}Id";

            var sql = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @{columnName}";
            parameters.Add(columnName, value);

            // Thêm bỏ qua hàng hiện tại (Nếu đang cập nhật)
            if (ignoreId.HasValue)
            {
                sql += $" AND {pkPropName} != '{ignoreId}'";
            }

            return sql;
        }


        /// <summary>
        /// Hàm tạo câu lệnh đếm số hàng bị trùng
        /// </summary>
        public static string BuildQueryStringCountDuplicate<TEntity>(Guid? ignoreId, string columnName, object value, out DynamicParameters parameters) where TEntity : BaseEntity
        {
            return BuildQueryStringCountDuplicate(typeof(TEntity), ignoreId, columnName, value, out parameters);
        }


        /// <summary>
        /// Hàm hỗ trợ build điều kiện lọc theo từ khóa Key
        /// </summary>
        public static string BuildKeySearchClaude<TEntity>(string? key, DynamicParameters parameters) where TEntity : BaseEntity
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            var entityType = typeof(TEntity);
            var mainTableName = entityType.Name;
            var stringProps = entityType.GetProperties()
                .Where(p => p.PropertyType == typeof(string) && !Attribute.IsDefined(p, typeof(NotMappedAttribute)))
                .Select(p => p.Name)
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
        /// Hàm hỗ trợ build phần phân trang (LIMIT ... OFFSET ...)
        /// </summary>
        public static string BuildPagingClaude(int page, int size, DynamicParameters parameters)
        {
            parameters.Add("Limit", size);
            parameters.Add("Offset", (page - 1) * size);
            return " LIMIT @Limit OFFSET @Offset";
        }


        /// <summary>
        /// Hàm hỗ trợ build toàn bộ phần mệnh đề WHERE từ GetRequest (bao gồm Ids, Filters, và từ khóa Key)
        /// </summary>
        public static string BuildWhereClaude<TEntity>(GetRequest request, DynamicParameters parameters) where TEntity : BaseEntity
        {
            var entityType = typeof(TEntity);
            var mainTableName = entityType.Name;
            var whereConditions = new List<string>();

            // 1. Lọc theo danh sách Ids (nếu có)
            if (request.Ids != null && request.Ids.Count > 0)
            {
                var pkPropName = $"{mainTableName}Id";
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
        /// Hàm hỗ trợ xây SQL phần điều kiện câu lệnh WHERE từ Filter
        /// </summary>
        public static string BuildWhereClaudeFromFilters<TEntity>(List<Filter>? filters, FilterGroupType groupType, DynamicParameters parameters)
        {
            if (filters == null || filters.Count == 0)
            {
                return "TRUE";
            }

            var entityType = typeof(TEntity);
            var columnNames = entityType.GetAllColumnProps();
            var conditions = new List<string>();

            foreach (var filter in filters)
            {
                // Tìm property khớp không phân biệt hoa thường
                var matchedProp = columnNames.FirstOrDefault(c => c.Equals(filter.Property, StringComparison.OrdinalIgnoreCase));
                if (matchedProp == null)
                {
                    throw new BadRequestException("Thuộc tính lọc không tồn tại");
                }
                
                // Cập nhật lại Property thành tên chuẩn PascalCase của C# để tạo câu SQL đúng cột
                filter.Property = matchedProp;

                // Kiểm tra toàn tử hợp lệ
                //var filterOperator = filter.Operator;
                if (!filter.Operator.IsValidOperator(filter.Type))
                {
                    throw new BadRequestException("Toán tử không hợp lệ");
                }

                // Xây điều kiện
                var condition = filter.Operator.ToSQLKeyword($"`{entityType.Name}`.`{filter.Property}`", $"{filter.Property}");

                // Thêm tham số
                if (filter.Type == FilterType.Number)
                {
                    parameters.Add($"@{filter.Property}", int.Parse(filter.Value.ToString()));
                }
                else if (filter.Type == FilterType.String)
                {
                    parameters.Add($"@{filter.Property}", filter.Value.ToString());
                }
                else if (filter.Type == FilterType.Bool)
                {
                    parameters.Add($"@{filter.Property}", bool.Parse(filter.Value.ToString()));
                }
                else
                {
                    throw new InvalidDataException("Kiểu lọc chưa hỗ trợ");
                }

                // Lưu điều kiện
                if (condition != null)
                {
                    conditions.Add(condition);
                }
            }

            // Gộp điều kiện (xuống dòng giữa các điều kiện lọc)
            if (groupType == FilterGroupType.And)
            {
                return string.Join(" AND ", conditions);
            }

            return string.Join(" OR ", conditions);
        }


        /// <summary>
        /// Hàm hỗ trợ build phần sắp xếp
        /// </summary>
        public static string BuildSortClaude<TEntity>(GetRequest? pagingRequest = null)
        {
            var mainTableName = typeof(TEntity).Name;

            // Ưu tiên chọn tiêu chí sắp xếp được gửi lên
            if (pagingRequest != null && 
                !string.IsNullOrEmpty(pagingRequest.SortBy) &&
                pagingRequest.IsAsc != null)
            {
                // Kiểm tra xem sortby có là trường hợp lệ hay không
                var columnNames = typeof(TEntity).GetAllColumnProps();
                if (columnNames.Contains(pagingRequest.SortBy))
                {
                    return $"ORDER BY `{mainTableName}`.`{pagingRequest.SortBy}` {(pagingRequest.IsAsc.Value ? "ASC" : "DESC")}";
                }
            }

            // Nếu ko gửi lên thì chọn mặc định đã được cấu hình
            var attr = typeof(TEntity).GetAttribute<SearchConfigAttribute>();
            if (attr != null)
            {
                return $"ORDER BY `{mainTableName}`.`{attr.DefaultSortBy}` {(attr.DefaultIsAsc ? "ASC" : "DESC")}";
            }

            // Nếu chưa cấu hình thì sort theo Id chính giảm dần
            return $"ORDER BY `{mainTableName}`.`ModifiedDate` DESC";
        }


        /// <summary>
        /// Hàm hỗ trợ build phần chọn trường trả về
        /// </summary>
        public static string BuildSelectClaude<TEntity, TResult>() where TEntity : BaseEntity
        {
            var entityType = typeof(TEntity);
            var resultType = typeof(TResult);

            var mainTableName = entityType.Name;
            var mainColumns = entityType.GetAllColumnProps();
            var resultProps = resultType.GetProperties();

            var selectColumns = new List<string>();
            var joinClauses = new List<string>();
            var joinedTables = new HashSet<Type>();

            foreach (var prop in resultProps)
            {
                // Bỏ qua nếu thuộc tính được đánh dấu là [NotMapped]
                if (Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                {
                    continue;
                }

                // Kiểm tra xem thuộc tính có ForeignTableAttribute hay không
                var foreignAttr = Attribute.GetCustomAttribute(prop, typeof(ForeignTableAttribute)) as ForeignTableAttribute;
                if (foreignAttr != null)
                {
                    var foreignType = foreignAttr.EntityType;
                    if (foreignType != null)
                    {
                        var foreignTableName = foreignType.Name;
                        var propName = prop.Name;
                        var columnNameInDb = !string.IsNullOrEmpty(foreignAttr.ColumnName) ? foreignAttr.ColumnName : propName;

                        // Lấy các cột của bảng ngoại
                        var foreignColumns = foreignType.GetAllColumnProps();
                        if (foreignColumns.Contains(columnNameInDb))
                        {
                            selectColumns.Add($"`{foreignTableName}`.`{columnNameInDb}` AS `{propName}`");

                            // Thêm câu lệnh JOIN nếu chưa được JOIN trước đó
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
                    // Thuộc tính thuộc về bảng chính TEntity
                    var propName = prop.Name;
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
