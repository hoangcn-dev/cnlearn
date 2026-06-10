using System.Text.RegularExpressions;

namespace HoangCN.Core.Common.Utils
{
    /// <summary>
    /// Tiện ích hỗ trợ tạo chuỗi slug thân thiện cho SEO từ văn bản tiếng Việt
    /// </summary>
    public static class SlugUtil
    {
        /// <summary>
        /// Tạo chuỗi slug (đường dẫn thân thiện) từ tiêu đề hoặc văn bản tiếng Việt
        /// </summary>
        /// <param name="text">Nội dung văn bản gốc</param>
        /// <returns>Chuỗi slug không dấu ngăn cách bằng dấu gạch ngang</returns>
        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            // Chuyển về chữ thường
            text = text.ToLowerInvariant();

            // Loại bỏ dấu tiếng Việt
            text = Regex.Replace(text, "[áàảãạâấầẩẫậăắằẳẵặ]", "a");
            text = Regex.Replace(text, "[éèẻẽẹêếềểễệ]", "e");
            text = Regex.Replace(text, "[íìỉĩị]", "i");
            text = Regex.Replace(text, "[óòỏõọôốồổỗộơớờởỡợ]", "o");
            text = Regex.Replace(text, "[úùủũụưứừửữự]", "u");
            text = Regex.Replace(text, "[ýỳỷỹỵ]", "y");
            text = Regex.Replace(text, "đ", "d");

            // Loại bỏ các ký tự đặc biệt khác ngoài chữ cái thường, số, khoảng trắng và gạch ngang
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Thay thế nhiều khoảng trắng và gạch ngang liền nhau thành 1 khoảng trắng duy nhất
            text = Regex.Replace(text, @"[\s-]+", " ").Trim();

            // Thay thế tất cả khoảng trắng thành dấu gạch ngang (-)
            text = text.Replace(' ', '-');

            return text;
        }
    }
}

