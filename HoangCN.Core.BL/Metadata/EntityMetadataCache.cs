using HoangCN.Common.Attributes;
using HoangCN.Common.Base;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace HoangCN.Core.BL.Metadata
{
    /// <summary>
    /// Lưu trữ thông tin chi tiết về từng thuộc tính của Entity hoặc DTO
    /// </summary>
    public class PropertyMetadata
    {
        public PropertyInfo PropertyInfo { get; set; } = null!;
        public string PropertyName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public bool IsPrimaryKey { get; set; }
        public bool IsNotMapped { get; set; }
        public RequiredAttribute? RequiredAttr { get; set; }
        public StringLengthAttribute? StringLengthAttr { get; set; }
        public CheckExistAttribute? CheckExistAttr { get; set; }
        public ForeignTableAttribute? ForeignTableAttr { get; set; }
    }

    /// <summary>
    /// Lưu trữ thông tin metadata của một Type (Entity hoặc DTO)
    /// </summary>
    public class EntityMetadata
    {
        public Type EntityType { get; set; } = null!;
        public string PrimaryKeyName { get; set; } = string.Empty;
        public SearchConfigAttribute? SearchConfigAttr { get; set; }
        public List<PropertyMetadata> Properties { get; set; } = new();
        public List<string> ColumnNames { get; set; } = new();
    }

    /// <summary>
    /// Bộ nhớ đệm tĩnh lưu trữ cấu trúc Metadata của Entity, loại bỏ việc dùng Reflection C# lặp đi lặp lại
    /// </summary>
    public static class EntityMetadataCache
    {
        private static readonly ConcurrentDictionary<Type, EntityMetadata> _cache = new();

        /// <summary>
        /// Lấy thông tin Metadata của một Type (tải từ bộ nhớ đệm hoặc phân tích lần đầu)
        /// </summary>
        public static EntityMetadata GetMetadata(Type type)
        {
            return _cache.GetOrAdd(type, t =>
            {
                var isEntity = t.IsSubclassOf(typeof(BaseEntity));
                
                var metadata = new EntityMetadata
                {
                    EntityType = t,
                    PrimaryKeyName = isEntity ? $"{t.Name}Id" : string.Empty,
                    SearchConfigAttr = t.GetCustomAttribute<SearchConfigAttribute>(true)
                };

                var columnNames = new List<string>();
                var props = t.GetProperties();
                
                foreach (var prop in props)
                {
                    var isNotMapped = prop.GetCustomAttribute<NotMappedAttribute>(true) != null;
                    
                    // Kiểm tra xem kiểu thuộc tính có phải kiểu cột dữ liệu DB (ValueType, string, byte[]) hay không
                    var propType = prop.PropertyType;
                    var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;
                    var isDbPrimitive = underlyingType.IsValueType || underlyingType == typeof(string) || underlyingType == typeof(byte[]);
                    if (!isDbPrimitive)
                    {
                        isNotMapped = true; // Loại trừ các class điều hướng hoặc list phức tạp
                    }

                    var displayName = prop.GetCustomAttribute<DisplayNameAttribute>(true)?.DisplayName 
                                      ?? prop.GetCustomAttribute<DisplayAttribute>(true)?.GetName() 
                                      ?? prop.Name;

                    var propMeta = new PropertyMetadata
                    {
                        PropertyInfo = prop,
                        PropertyName = prop.Name,
                        DisplayName = displayName,
                        IsPrimaryKey = prop.GetCustomAttribute<KeyAttribute>(true) != null || prop.Name == metadata.PrimaryKeyName,
                        IsNotMapped = isNotMapped,
                        RequiredAttr = prop.GetCustomAttribute<RequiredAttribute>(true),
                        StringLengthAttr = prop.GetCustomAttribute<StringLengthAttribute>(true),
                        CheckExistAttr = prop.GetCustomAttribute<CheckExistAttribute>(true),
                        ForeignTableAttr = prop.GetCustomAttribute<ForeignTableAttribute>(true)
                    };
                    
                    metadata.Properties.Add(propMeta);

                    // Nếu là cột thực tế trong database (không bị đánh dấu NotMapped)
                    if (!isNotMapped)
                    {
                        columnNames.Add(prop.Name);
                    }
                }
                
                metadata.ColumnNames = columnNames;
                return metadata;
            });
        }
    }
}
