using HoangCN.Common.Base;
using HoangCN.Common.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.Common.Utils
{
    public static class TypeUtil
    {
        /// <summary>
        /// Lấy ra toàn bộ tên các thuộc tính (tương ứng các cột) của entity, ngoại trừ các thuộc tính được đánh dấu là [NotMapped] 
        /// </summary>
        public static List<string> GetAllColumnProps(this Type entityType)
        {
            entityType.MustExtendFromBaseEntity();

            var columns = new List<string>();
            var props = entityType.GetProperties();
            if (props?.Length > 0)
            {
                var propNames = props
                    .Where(p => !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                                && !p.PropertyType.IsSubclassOf(typeof(BaseEntity))
                                && !p.PropertyType.GetInterfaces().Any(i => i.IsGenericType 
                                    && i.GetGenericTypeDefinition() == typeof(IEnumerable<>) 
                                    && i.GetGenericArguments()[0].IsSubclassOf(typeof(BaseEntity))))
                    .Select(p => p.Name);
                columns.AddRange(propNames);
            }
            return columns;
        }

        /// <summary>
        /// Đảm bảo entityType là kiểu kế thừa từ BaseEntity
        /// </summary>
        private static void MustExtendFromBaseEntity(this Type entityType)
        {
            if (!entityType.IsSubclassOf(typeof(BaseEntity)))
            {
                throw new ServerErrorException("Thao tác trên Object không phải là BaseEntity");
            }
        }
    }
}
