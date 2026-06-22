namespace HoangCN.LearnMS.DTOs
{
    public class SavedDto
    {
        /// <summary>
        /// Id của đối tượng được lưu
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// Kiểu nội dung lưu
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
