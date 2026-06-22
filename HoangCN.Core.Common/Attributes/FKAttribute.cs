using System.ComponentModel.DataAnnotations;

namespace HoangCN.Core.Common.Attributes
{
    /// <summary>
    /// Đánh dấu trường này là khóa ngoại
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FKAttribute : ValidationAttribute
    {
        /// <summary>
        /// Kiểu entity đại diện cho bảng ngoại
        /// </summary>
        public Type TargetEntity { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            HashSet<Type> validTypes = [typeof(Guid), typeof(string), typeof(int), typeof(long)];
            if (!validTypes.Contains(value.GetType()))
            {
                return new ValidationResult($"FKAttribute hiện tại chỉ hỗ trợ kiểu dữ liệu: {string.Join(", ", validTypes.Select(t => t.Name))}.");
            }
            return ValidationResult.Success;
        }
    }
}
