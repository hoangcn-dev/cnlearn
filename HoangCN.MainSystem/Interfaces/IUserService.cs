using HoangCN.Core.BL.Interfaces;
using HoangCN.MainSystem.DTOs;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Requests;

namespace HoangCN.MainSystem.Interfaces
{
    /// <summary>
    /// Giao diện định nghĩa các nghiệp vụ quản lý người dùng và xác thực tài khoản
    /// </summary>
    public interface IUserService : IBaseBL<User>
    {
        /// <summary>
        /// Đăng ký tài khoản người dùng mới vào hệ thống
        /// </summary>
        /// <param name="request">Yêu cầu đăng ký chứa thông tin tài khoản, mật khẩu và thông tin cá nhân</param>
        Task SignUp(SignUpRequest request);

        /// <summary>
        /// Đăng nhập tài khoản vào hệ thống để cấp mã Token/Cookie phiên làm việc
        /// </summary>
        /// <param name="request">Yêu cầu đăng nhập chứa Email/Username và mật khẩu</param>
        Task SignIn(SignInRequest request);

        /// <summary>
        /// Lấy thông tin chi tiết về phiên làm việc của người dùng hiện tại sau khi đăng nhập thành công
        /// </summary>
        /// <param name="userId">ID của người dùng cần lấy thông tin</param>
        /// <returns>Thông tin phiên đăng nhập của người dùng (tên hiển thị, email, quyền hạn)</returns>
        Task<LoginSessionInfoDto> GetLoginSessionInfo(Guid userId);

        /// <summary>
        /// Xử lý yêu cầu khôi phục mật khẩu khi người dùng quên mật khẩu đăng nhập
        /// </summary>
        /// <param name="request">Yêu cầu khôi phục chứa địa chỉ Email đã đăng ký</param>
        Task ForgotPassword(ForgotPasswordRequest request);

        /// <summary>
        /// Đổi mật khẩu của người dùng sang mật khẩu mới
        /// </summary>
        /// <param name="userId">ID của người dùng thực hiện đổi mật khẩu</param>
        /// <param name="request">Yêu cầu đổi mật khẩu chứa mật khẩu cũ và mật khẩu mới</param>
        Task ChangePassword(Guid userId, ChangePasswordRequest request);

        /// <summary>
        /// Đăng xuất tài khoản hiện tại ra khỏi hệ thống, hủy bỏ token và phiên làm việc
        /// </summary>
        Task SignOut();
    }
}
