using Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Core.Utilities
{
    public class StringUtilTests
    {
        [Fact]
        public void ToSlug_WithNormalString_ReturnValidSlug()
        {
            var input = "Xin chào tất cả các bạn, tôi tên là Hoàng Cao Nguyên, sinh ngày 14-02-2004!";
            var result = StringUtil.ToSlug(input);
            Assert.Equal("xin-chao-tat-ca-cac-ban-toi-ten-la-hoang-cao-nguyen-sinh-ngay-14-02-2004", result);
        }

        [Theory]
        [InlineData("Xin chào tất cả các bạn", "xin-chao-tat-ca-cac-ban")]
        [InlineData("   Học   Lập  Trình   C#   ", "hoc-lap-trinh-c")]
        [InlineData("Đường đến thành công!", "duong-den-thanh-cong")]
        [InlineData("Công nghệ thông tin 2025", "cong-nghe-thong-tin-2025")]
        [InlineData("Hello---World!!!", "hello-world")]
        [InlineData("Ăn cơm, uống nước, nghỉ ngơi.", "an-com-uong-nuoc-nghi-ngoi")]
        [InlineData("Nắng ☀ và mưa 🌧", "nang-va-mua")]
        [InlineData("Hoàng_Cao_Nguyên", "hoang-cao-nguyen")]
        [InlineData("  ---Xin---Chào---  ", "xin-chao")]
        [InlineData("Tôi tên là Hoàng Cao Nguyên, sinh ngày 14-02-2004!",
                    "toi-ten-la-hoang-cao-nguyen-sinh-ngay-14-02-2004")]
        public void ToSlug_Should_Return_ValidSlug(string input, string expected)
        {
            var result = StringUtil.ToSlug(input);
            Assert.Equal(expected, result);
        }
    }
}
