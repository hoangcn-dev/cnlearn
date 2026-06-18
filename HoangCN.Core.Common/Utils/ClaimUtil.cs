using HoangCN.Core.Common.Exceptions;
using System.Security.Claims;

namespace HoangCN.Core.Common.Utils
{
    public class ClaimUtil
    {
        /// <summary>
        /// Lấy giá trị của claim = key từ ClaimsPrincipal
        /// </summary>
        public static string GetByKey(ClaimsPrincipal claims, string key)
        {
            var value = claims!.Claims.FirstOrDefault(c => c.Type == key)?.Value
                ?? throw new UnauthorizedException("Thông tin đăng nhập không hợp lệ");
            return value;
        }


        /// <summary>
        /// Lấy tên tài khoản người dùng từ ClaimsPrincipal
        /// </summary>
        public static string GetUserName(ClaimsPrincipal? claims)
        {
            if (claims == null)
            {
                throw new InvalidOperationException("Thao tác chỉnh sửa đang được thực hiện bởi người dùng không xác định");
            }
            var userName = claims!.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            return userName!;
        }

        /// <summary>
        /// Lấy UserId từ ClaimsPrincipal
        /// </summary>
        public static Guid? GetUserId(ClaimsPrincipal? claims)
        {
            var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedException("Vui lòng đăng nhập để thực hiện chức năng này");
            }
            return userId;
        }

    }
}
