using Dapper;
using HoangCN.Core.Common.Attributes;
using HoangCN.Core.Common.Enums;
using System.ComponentModel.DataAnnotations;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.DL.Interfaces;
using System.Collections;
using HoangCN.Core.Common.Metadata;
using HoangCN.Core.Common.Base;

namespace HoangCN.Core.DL.Utils
{
    public class ValidateUtil
    {
        /// <summary>
        /// CheckExist: Kiểm tra sự tồn tại hoặc không trùng lặp của các bản ghi trong cơ sở dữ liệu
        /// dựa trên các thuộc tính [CheckExist] đã được định nghĩa trong lớp thực thể.
        /// </summary>
        public static async Task CheckExist<T>(IEnumerable<T> items, IBaseReadDL dl)
        {
            if (items == null || !items.Any()) return;

            var itemList = items.ToList();
            var metadata = EntityMetadataCache.GetMetadata(typeof(T));
            var pkPropName = metadata.PrimaryKeyName;

            foreach (var prop in metadata.Properties)
            {
                var checkExistAttr = prop.CheckExistAttr;
                if (checkExistAttr == null) continue;

                var targetEntityType = checkExistAttr.TargetEntity ?? metadata.EntityType;
                var targetTableName = targetEntityType.Name;
                var queryColumnName = prop.PropertyName;

                if (checkExistAttr.TargetEntity != null)
                {
                    queryColumnName = $"{checkExistAttr.TargetEntity.Name}Id";
                }

                // Thu thập các giá trị cần kiểm tra từ dữ liệu đầu vào
                var valuesToCheck = itemList
                    .Select(e => prop.PropertyInfo.GetValue(e))
                    .Where(v => v != null && !(v is Guid g && g == Guid.Empty) && !(v is string s && string.IsNullOrEmpty(s)))
                    .Distinct()
                    .ToList();

                if (valuesToCheck.Count == 0) continue;

                var param = new DynamicParameters();
                param.Add("Values", valuesToCheck);

                if (checkExistAttr.TargetEntity == null)
                {
                    // Kiểm tra trùng lặp cột trên chính bảng này (loại trừ trường hợp so trùng với chính hàng đang sửa)
                    var sql = BuildSQLUtil.BuildQueryStringCheckDuplicate(targetTableName, pkPropName, queryColumnName);
                    var existingRecords = await dl.ExecuteQueryText<dynamic>(sql, param);

                    foreach (var item in itemList)
                    {
                        var val = prop.PropertyInfo.GetValue(item);
                        if (val == null) continue;

                        Guid currentId = Guid.Empty;
                        if (!string.IsNullOrEmpty(pkPropName))
                        {
                            var pkProp = metadata.Properties.FirstOrDefault(p => p.PropertyName == pkPropName);
                            if (pkProp != null)
                            {
                                Guid.TryParse(pkProp.PropertyInfo.GetValue(item)?.ToString(), out currentId);
                            }
                        }

                        var isDuplicate = existingRecords.Any(r => 
                        {
                            var dict = (IDictionary<string, object>)r;
                            var recordVal = dict[queryColumnName]?.ToString();
                            var recordId = Guid.Parse(dict[pkPropName].ToString()!);
                            return recordVal == val.ToString() && recordId != currentId;
                        });

                        if (isDuplicate)
                        {
                            var msg = string.Format(checkExistAttr.ErrorMessage ?? "Trường {0} đã tồn tại trong hệ thống.", prop.DisplayName);
                            throw new BadRequestException(msg);
                        }
                    }
                }
                else
                {
                    // Kiểm tra khóa ngoại trên bảng khác có tồn tại khóa chính tương ứng
                    var sql = BuildSQLUtil.BuildQueryStringCheckFkExists(targetTableName, queryColumnName);
                    var existingRecords = await dl.ExecuteQueryText<dynamic>(sql, param);
                    var existingValues = existingRecords
                        .Select(r => 
                        {
                            var dict = (IDictionary<string, object>)r;
                            return dict.Values.FirstOrDefault()?.ToString();
                        })
                        .Where(v => v != null)
                        .ToHashSet()!;

                    foreach (var item in itemList)
                    {
                        var val = prop.PropertyInfo.GetValue(item);
                        if (val == null) continue;

                        var exists = existingValues.Contains(val.ToString()!);
                        if (checkExistAttr.MustExist && !exists)
                        {
                            var msg = string.Format(checkExistAttr.ErrorMessage ?? "Trường {0} không tồn tại trong hệ thống.", prop.DisplayName);
                            throw new BadRequestException(msg);
                        }
                        else if (!checkExistAttr.MustExist && exists)
                        {
                            var msg = string.Format(checkExistAttr.ErrorMessage ?? "Trường {0} đã tồn tại trong hệ thống.", prop.DisplayName);
                            throw new BadRequestException(msg);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của các khóa chính (Primary Key) trong cơ sở dữ liệu khi cập nhật.
        /// </summary>
        public static async Task ValidatePkExists<T>(IEnumerable<T> items, IBaseReadDL dl)
        {
            if (items == null || !items.Any()) return;

            var itemList = items.ToList();
            var metadata = EntityMetadataCache.GetMetadata(typeof(T));
            var pkPropName = metadata.PrimaryKeyName;

            if (string.IsNullOrEmpty(pkPropName)) return;

            var pkProp = metadata.Properties.FirstOrDefault(p => p.PropertyName == pkPropName);
            if (pkProp == null) return;

            var idsToCheck = itemList.Select(e =>
            {
                Guid.TryParse(pkProp.PropertyInfo.GetValue(e)?.ToString(), out Guid id);
                return id;
            }).Where(id => id != Guid.Empty).Distinct().ToList();

            if (idsToCheck.Count == 0) return;

            var sql = BuildSQLUtil.BuildQueryStringCheckPkExists(metadata.EntityType.Name, pkPropName);
            var param = new DynamicParameters();
            param.Add("Ids", idsToCheck);

            var existingIds = (await dl.ExecuteQueryText<Guid>(sql, param)).ToHashSet();

            foreach (var item in itemList)
            {
                Guid.TryParse(pkProp.PropertyInfo.GetValue(item)?.ToString(), out Guid id);
                if (id != Guid.Empty && !existingIds.Contains(id))
                {
                    throw new BadRequestException($"Đối tượng {metadata.EntityType.Name} có mã {id} được sửa đổi không tồn tại.");
                }
            }
        }

        /// <summary>
        /// Kiểm tra null và đồ dài chuỗi dựa trên các thuộc tính [Required] và [StringLength] đã được định nghĩa trong lớp thực thể.
        /// </summary>
        public static void CommonValidate(IEnumerable items, params string[] excepts)
        {
            string[] auditProp = [
                nameof(BaseEntity.CreatedBy),
                nameof(BaseEntity.CreatedDate),
                nameof(BaseEntity.ModifiedBy),
                nameof(BaseEntity.ModifiedDate),
            ];

            // 1. Kiểm tra các điều kiện trong bộ nhớ (Required, StringLength) trước - Hoàn toàn không gọi DB
            foreach (var item in items)
            {
                var metadata = EntityMetadataCache.GetMetadata(item.GetType());
                var pkPropName = metadata.PrimaryKeyName;

                foreach (var prop in metadata.Properties)
                {
                    // Bỏ qua kiểm tra các trường audit mặc định
                    if (auditProp.Contains(prop.PropertyName)) continue;

                    if (excepts.Contains(prop.PropertyName)) continue;

                    var v = prop.PropertyInfo.GetValue(item);

                    // Kiểm tra null hoặc trống [Required]
                    var isNullOrEmpty = prop.RequiredAttr != null && (v is null ||
                                        v is string strV && string.IsNullOrEmpty(strV));
                    if (isNullOrEmpty)
                    {
                        var msg = !string.IsNullOrEmpty(prop.RequiredAttr.ErrorMessage)
                            ? prop.RequiredAttr.FormatErrorMessage(prop.DisplayName)
                            : $"Trường {prop.DisplayName} không được phép để trống.";
                        throw new BadRequestException(msg);
                    }

                    // Kiểm tra độ dài [StringLength]
                    if (prop.StringLengthAttr != null && v is string sValue)
                    {
                        if (sValue.Length < prop.StringLengthAttr.MinimumLength ||
                            sValue.Length > prop.StringLengthAttr.MaximumLength)
                        {
                            var msg = !string.IsNullOrEmpty(prop.StringLengthAttr.ErrorMessage)
                                ? prop.StringLengthAttr.FormatErrorMessage(prop.DisplayName)
                                : $"Độ dài của trường {prop.DisplayName} không hợp lệ.";
                            throw new BadRequestException(msg);
                        }
                    }

                    // Kiểm tra các ValidationAttribute khác (EmailAddress, Phone, RegularExpression, Range, v.v.)
                    foreach (var attr in prop.ValidationAttrs)
                    {
                        if (attr is RequiredAttribute || attr is StringLengthAttribute) continue;

                        var hasValue = v != null && !(v is string s && string.IsNullOrEmpty(s));
                        if (hasValue && !attr.IsValid(v))
                        {
                            var msg = !string.IsNullOrEmpty(attr.ErrorMessage)
                                ? attr.FormatErrorMessage(prop.DisplayName)
                                : $"Trường {prop.DisplayName} không hợp lệ.";
                            throw new BadRequestException(msg);
                        }
                    }

                    // Đệ quy hết các đối tượng con (nếu có)
                    if (v == null || v is string) continue; // Bỏ qua null và string
                    if (v is IEnumerable subItems)
                    {
                        CommonValidate(subItems, excepts);
                    }
                    else if (v.GetType().IsClass)
                    {
                        CommonValidate(new [] { v }, excepts);
                    }
                }

            }

        }

    }
}
