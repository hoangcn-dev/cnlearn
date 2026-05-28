using HoangCN.Common.Exceptions;
using System.Collections;

namespace HoangCN.Common.Utils
{
    public static class ObjectUtil
    {
        /// <summary>
        /// Lấy 1 Attribute của một Type bất kì
        /// </summary>
        public static TAttribute? GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            if (type == null) return null;

            return type.GetCustomAttributes(typeof(TAttribute), true)
                .FirstOrDefault() as TAttribute;
        }

        /// <summary>
        /// Hàm lấy giá trị thuộc tính của đối tượng theo tên property
        /// </summary>
        public static object? GetValueByPropName(this object obj, string propName)
        {
            object? propValue = null;
            if (obj != null && !string.IsNullOrEmpty(propName))
            {
                if (obj is IDictionary<string, object> dict)
                {
                    dict.TryGetValue(propName, out propValue);
                }
                else
                {
                    var prop = obj.GetType().GetProperty(propName);
                    if (prop != null)
                    {
                        propValue = prop.GetValue(obj, null);
                    }
                }
            }
            return propValue;
        }

        /// <summary>
        /// Hàm set giá trị cho đối tương bất kì theo tên property
        /// </summary>
        public static void SetValueByPropName(this object obj, string propName, object value)
        {
            if (obj != null && !string.IsNullOrEmpty(propName))
            {
                if (obj is IDictionary<string, object> dict)
                {
                    dict.TryGetValue(propName, out var oldValue);
                    if (oldValue != null && oldValue.GetType() == value.GetType())
                    {
                        dict[propName] = value;
                        return;
                    }
                }
                else if (obj is IEnumerable)
                {
                    return;
                }
                else
                {
                    var prop = obj.GetType().GetProperty(propName);
                    if (prop != null && prop.PropertyType == value.GetType())
                    {
                        prop.SetValue(obj, value);
                        return;
                    }
                }
            }

            throw new ServerErrorException("Gán giá trị cho đối tượng runtime thất bại");
        }
    }
}
