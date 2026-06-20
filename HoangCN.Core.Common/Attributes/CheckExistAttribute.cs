namespace HoangCN.Core.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckExistAttribute : Attribute
    {
        public bool MustExist { get; set; }

        ///// <summary>
        ///// Có giới hạn phạm vi check trong tập con của cha hay không, nếu không check trên toàn bộ DB
        ///// </summary>
        //public bool IsCheckWithParent { get; set; } = false;
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Kiểu thực thể đích cần kiểm tra sự tồn tại (dành cho Khóa ngoại)
        /// </summary>
        public Type? TargetEntity { get; set; }
    }
}

