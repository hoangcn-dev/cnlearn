using HoangCN.BL.Base;
using HoangCN.BL.Interfaces;
using HoangCN.Common.Exceptions;
using HoangCN.Common.Model.Entities;
using HoangCN.Common.Model.Requests;
using HoangCN.Common.Enums;
using HoangCN.DL.Interfaces;
using HoangCN.UserManagement.DTOs;
using HoangCN.UserManagement.Enums;
using HoangCN.UserManagement.Interfaces;
using HoangCN.UserManagement.Requests;
using HoangCN.UserManagement.Utils;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using HoangCN.MainSystem.Interfaces;
using HoangCN.MainSystem.Models;
using Microsoft.Extensions.Options;

namespace HoangCN.UserManagement.Services
{
    public class UserService : BaseBL<User>, IUserService
    {
        private readonly IHttpContextAccessor _accesstor;
        private readonly JwtUtil _jwtUtil;
        private readonly IBaseBL<Role> _roleBL;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly EmailSettings _emailSettings;
        private readonly IRedisService _redisService;

        private class TempPasswordCache
        {
            public string PasswordHash { get; set; } = string.Empty;
            public string PasswordSalt { get; set; } = string.Empty;
        }

        public UserService(
            IBaseDL baseDL,
            IHttpContextAccessor accesstor,
            JwtUtil jwtUtil,
            IBaseBL<Role> roleBL,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            IOptions<EmailSettings> emailOptions,
            IRedisService redisService) : base(baseDL)
        {
            _accesstor = accesstor;
            _jwtUtil = jwtUtil;
            _roleBL = roleBL;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _emailSettings = emailOptions?.Value ?? throw new ArgumentNullException(nameof(emailOptions));
            _redisService = redisService;
        }

        public async Task<Guid> CheckAuth(ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedException("Vui lòng đăng nhập để thực hiện chức năng này");
            }
            return await Task.FromResult(userId);
        }

        public async Task<LoginSessionInfoDto> GetLoginSessionInfo(Guid userId)
        {
            var info = await GetById<LoginSessionInfoDto>(userId);
            if (info == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại trong hệ thống");
            }
            return info;
        }

        public async Task SignUp(SignUpRequest request)
        {
            // 1. Kiểm tra tài khoản hoặc email đã tồn tại hay chưa
            var existingUsers = await GetByCondition<User>(u => u.UserName == request.UserName || u.Email == request.Email);
            if (existingUsers != null && existingUsers.Count > 0)
            {
                throw new BadRequestException("Tên tài khoản hoặc email đã tồn tại trong hệ thống");
            }

            // 2. Lấy RoleId mặc định cho người dùng đăng ký (User)
            var roles = await _roleBL.GetByCondition<Role>(r => r.RoleName == RoleNames.User.ToString());
            var defaultRole = roles.FirstOrDefault();

            if (defaultRole == null)
            {
                throw new ServerErrorException($"Không tìm thấy vai trò người dùng mặc định '{RoleNames.User}' trong hệ thống.");
            }

            // 3. Mã hóa mật khẩu
            var (hash, salt) = PasswordUtil.HashPassword(request.Password);

            // 4. Tạo đối tượng User mới
            var user = new User
            {
                UserName = request.UserName,
                UserCode = "U_" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                Email = request.Email,
                DisplayName = request.DisplayName,
                Password = hash,
                PasswordSalt = salt,
                RoleId = defaultRole.RoleId,
                IsActive = true,
                IsVerified = false
            };

            // 5. Lưu xuống cơ sở dữ liệu
            await Save(new List<User> { user });
        }

        public async Task SignIn(SignInRequest request)
        {
            // Tìm user theo tên tài khoản / email và lấy kèm RoleName bằng cách dùng GetByCondition của lớp base
            var users = await GetByCondition<UserAuthDto>(u => u.UserName == request.EmailOrUserName || u.Email == request.EmailOrUserName);

            // Không tìm thấy user
            if (users == null || users.Count == 0)
            {
                throw new UnauthorizedException("Sai tài khoản hoặc mật khẩu");
            }

            var userAuth = users[0];

            // Mật khẩu không chính xác
            var isPasswordValid = PasswordUtil.VerifyPassword(request.Password, userAuth.Password, userAuth.PasswordSalt);
            if (!isPasswordValid)
            {
                // Fallback check Redis for temporary password
                var redisKey = $"{userAuth.UserId}_temppass";
                var cachedTempPass = await _redisService.GetAsync<TempPasswordCache>(redisKey);
                if (cachedTempPass == null || !PasswordUtil.VerifyPassword(request.Password, cachedTempPass.PasswordHash, cachedTempPass.PasswordSalt))
                {
                    throw new UnauthorizedException("Sai tài khoản hoặc mật khẩu");
                }
            }

            // Kiểm tra nếu đang bị khóa thì không được đăng nhập
            if (!userAuth.IsActive)
            {
                throw new UnauthorizedException("Tài khoản của bạn đã bị khóa, vui lòng liên hệ admin để được hỗ trợ");
            }

            // Chuyển đổi sang đối tượng User thuần túy để tránh lỗi runtime khi lưu (GetType().Name sẽ trả về đúng "User")
            var user = new User
            {
                UserId = userAuth.UserId,
                UserName = userAuth.UserName,
                UserCode = userAuth.UserCode,
                DisplayName = userAuth.DisplayName,
                Email = userAuth.Email,
                Password = userAuth.Password,
                PasswordSalt = userAuth.PasswordSalt,
                IsActive = userAuth.IsActive,
                IsVerified = userAuth.IsVerified,
                AvatarImageFileId = userAuth.AvatarImageFileId,
                RoleId = userAuth.RoleId,
                LastLogin = DateTime.UtcNow,
                CreatedBy = userAuth.CreatedBy,
                CreatedDate = userAuth.CreatedDate,
                ModifiedBy = userAuth.ModifiedBy,
                ModifiedDate = DateTime.UtcNow
            };
            
            await Save(new List<User> { user });

            // Sinh JWT Token bằng cách khởi tạo Role inline từ RoleName có sẵn trong DTO
            var role = new Role { RoleName = userAuth.RoleName };
            var token = _jwtUtil.IssueToken(userAuth, role, true);

            // Lưu JWT Token vào HttpContext cookies để client sử dụng
            var httpContext = _accesstor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Response.Cookies.Append("aToken", token.AccessToken, new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = token.ExpireAt
                });
            }
        }

        /// <summary>
        /// Xử lý khôi phục mật khẩu: Tạo mật khẩu ngẫu nhiên mới, cập nhật vào DB, và gửi mail đến user
        /// </summary>
        public async Task ForgotPassword(ForgotPasswordRequest request)
        {
            // Kiểm tra tham số đầu vào theo Rule 1
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new BadRequestException("Email không được để trống.");
            }

            // 1. Tìm kiếm User theo Email đăng ký
            var users = await GetByCondition<User>(u => u.Email == request.Email);
            if (users == null || users.Count == 0)
            {
                throw new NotFoundException("Địa chỉ email này chưa được đăng ký trong hệ thống.");
            }

            var user = users[0];

            // Kiểm tra nếu tài khoản đang bị khóa
            if (!user.IsActive)
            {
                throw new BadRequestException("Tài khoản của bạn hiện đang bị khóa. Vui lòng liên hệ Admin.");
            }

            // 2. Sinh mật khẩu tạm thời ngẫu nhiên bằng hàm tiện ích bảo mật
            var tempPassword = PasswordUtil.GenerateRandomPassword(8);

            // 3. Mã hóa mật khẩu tạm thời mới và lưu vào Redis để tránh ảnh hưởng tài khoản chính thức
            var (hash, salt) = PasswordUtil.HashPassword(tempPassword);
            var expireMin = _emailSettings.TemporaryPasswordExpireMin > 0 ? _emailSettings.TemporaryPasswordExpireMin : 15;
            
            var redisKey = $"{user.UserId}_temppass";
            var cacheData = new TempPasswordCache
            {
                PasswordHash = hash,
                PasswordSalt = salt
            };
            await _redisService.SetAsync(redisKey, cacheData, TimeSpan.FromMinutes(expireMin));

            // 4. Lấy cấu hình template gửi email quên mật khẩu
            var template = await _emailTemplateService.GetTemplateAsync("forgot_password");

            // 5. Thay thế các biến động trong Template
            var expireTime = $"{expireMin} phút";

            var emailSubject = template.Subject
                .Replace("{{DisplayName}}", user.DisplayName)
                .Replace("{{TemporaryPassword}}", tempPassword)
                .Replace("{{ExpireTime}}", expireTime);

            var emailBody = template.Content
                .Replace("{{DisplayName}}", user.DisplayName)
                .Replace("{{TemporaryPassword}}", tempPassword)
                .Replace("{{ExpireTime}}", expireTime);

            // 6. Thực hiện gửi mail bất đồng bộ
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody, true);
        }

        /// <summary>
        /// Thực hiện đổi mật khẩu cho người dùng hiện tại
        /// </summary>
        public async Task ChangePassword(Guid userId, ChangePasswordRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                throw new BadRequestException("Mật khẩu hiện tại không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new BadRequestException("Mật khẩu mới không được để trống.");
            }

            var user = await GetById<User>(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin tài khoản người dùng.");
            }

            // Kiểm tra mật khẩu hiện tại bằng tiện ích mã hóa
            var isMatched = PasswordUtil.VerifyPassword(request.CurrentPassword, user.Password, user.PasswordSalt);
            bool isTempPasswordUsed = false;
            
            if (!isMatched)
            {
                // Fallback check Redis for temporary password
                var redisKey = $"{user.UserId}_temppass";
                var cachedTempPass = await _redisService.GetAsync<TempPasswordCache>(redisKey);
                if (cachedTempPass == null || !PasswordUtil.VerifyPassword(request.CurrentPassword, cachedTempPass.PasswordHash, cachedTempPass.PasswordSalt))
                {
                    throw new BadRequestException("Mật khẩu hiện tại không chính xác. Vui lòng kiểm tra lại.");
                }
                isTempPasswordUsed = true;
            }

            // Mã hóa mật khẩu mới và gán lại cho user
            var (hash, salt) = PasswordUtil.HashPassword(request.NewPassword);
            user.Password = hash;
            user.PasswordSalt = salt;
            user.ModifiedDate = DateTime.UtcNow;

            // Lưu thay đổi vào DB
            await Save(new List<User> { user });

            // Nếu đã sử dụng mật khẩu tạm thời thành công để đổi sang mật khẩu chính thức, xóa mật khẩu tạm thời khỏi Redis
            if (isTempPasswordUsed)
            {
                var redisKey = $"{user.UserId}_temppass";
                await _redisService.DeleteAsync(redisKey);
            }
        }

        /// <summary>
        /// Tiền xử lý trước khi lưu: Tự sinh mã tài khoản và mã hóa mật khẩu cho tài khoản mới
        /// </summary>
        protected override async Task BeforeSave(List<User> entities)
        {
            await base.BeforeSave(entities);

            foreach (var user in entities)
            {
                if (user.State == ModelState.Insert)
                {
                    // Tự động sinh mã tài khoản (UserCode) nếu chưa nhập
                    if (string.IsNullOrWhiteSpace(user.UserCode))
                    {
                        user.UserCode = "U_" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                    }

                    // Tự động mã hóa mật khẩu khi lưu
                    if (!string.IsNullOrWhiteSpace(user.Password))
                    {
                        var (hash, salt) = PasswordUtil.HashPassword(user.Password);
                        user.Password = hash;
                        user.PasswordSalt = salt;
                    }
                }
            }

            await base.BeforeSave(entities);
        }
    }
}
