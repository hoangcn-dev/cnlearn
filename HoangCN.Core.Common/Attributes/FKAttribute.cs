using System.ComponentModel.DataAnnotations;

namespace HoangCN.Core.Common.Attributes
{
    /// <summary>
    /// Đánh dấu trường này là khóa ngoại
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FKAttribute : Attribute
    {
        /// <summary>
        /// Kiểu entity đại diện cho bảng ngoại
        /// </summary>
        public Type TargetEntity { get; set; }
    }
}
