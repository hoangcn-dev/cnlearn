namespace HoangCN.Common.Model.Requests
{
    /// <summary>
    /// Yêu cầu xóa chung
    /// </summary>
    public class DeleteRequest
    {
        /// <summary>
        /// Danh sách ID của đối tượng cần xóa
        /// </summary>
        public List<Guid> Ids { get; set; }
    }
}
