namespace HoangCN.MainSystem.Interfaces
{
    /// <summary>
    /// Giao diện dịch vụ gửi mail
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email bất đồng bộ
        /// </summary>
        /// <param name="toEmail">Địa chỉ email nhận</param>
        /// <param name="subject">Tiêu đề email</param>
        /// <param name="body">Nội dung email</param>
        /// <param name="isHtml">Nội dung có định dạng HTML hay không</param>
        /// <returns></returns>
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Gửi email bất đồng bộ kèm tệp đính kèm
        /// </summary>
        /// <param name="toEmail">Địa chỉ email nhận</param>
        /// <param name="subject">Tiêu đề email</param>
        /// <param name="body">Nội dung email</param>
        /// <param name="attachments">Danh sách tệp đính kèm (Tên tệp và nội dung byte[])</param>
        /// <param name="isHtml">Nội dung có định dạng HTML hay không</param>
        /// <returns></returns>
        Task SendEmailWithAttachmentsAsync(
            string toEmail, 
            string subject, 
            string body, 
            IEnumerable<(string FileName, byte[] Content)> attachments, 
            bool isHtml = true);
    }
}
