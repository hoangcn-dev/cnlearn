using HoangCN.Core.Common.Base;
using System.ComponentModel;
using System.Reflection;

namespace HoangCN.Core.Common.Utils
{
    public static class ReflectionUtil
    {
        public static Guid GetId<TEntity>(this TEntity entity) where TEntity : BaseEntity
        {
            var rawId = entity.GetType().GetValueByPropName(GetIdPropName<TEntity>())
                ?? throw new InvalidOperationException($"Thực thể {typeof(TEntity).GetType().Name} không tồn tại khóa ID hợp lệ");
            _ = Guid.TryParse(rawId.ToString(), out Guid id);
            return id;
        }

        public static string GetIdPropName<TEntity>() where TEntity : BaseEntity
        {
            var tableName = typeof(TEntity).GetType().Name;
            return $"{tableName}Id";
                ; ;
        }

        public static string GetPropDisplayName(this PropertyInfo prop)
        {
            var displayNameAttr = prop.GetCustomAttribute<DisplayNameAttribute>(true);
            return displayNameAttr?.DisplayName ?? prop.Name;
        }

        public static string GetEntityDisplayName<TEntity>() where TEntity : BaseEntity
        {
            var displayNameAttr = typeof(TEntity).GetCustomAttribute<DisplayNameAttribute>(true);
            return displayNameAttr?.DisplayName ?? typeof(TEntity).Name;
        }
    }
}
