namespace HoangCN.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckExistAttribute : Attribute
    {
        public bool MustExist { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Kiểu thực thể đích cần kiểm tra sự tồn tại (dành cho Khóa ngoại)
        /// </summary>
        public Type? TargetEntity { get; set; }
    }
}
