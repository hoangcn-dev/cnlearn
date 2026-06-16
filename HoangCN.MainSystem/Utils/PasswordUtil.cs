using System.Security.Cryptography;

namespace HoangCN.MainSystem.Utils
{
    public class PasswordUtil
    {
        private const int SaltSize = 16; // 128-bit
        private const int HashSize = 32; // 256-bit
        private const int Iterations = 100_000;

        public static (string Hash, string Salt) HashPassword(string password)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256);

            byte[] hashBytes = pbkdf2.GetBytes(HashSize);

            return (
                Convert.ToBase64String(hashBytes),
                Convert.ToBase64String(saltBytes)
            );
        }

        public static bool VerifyPassword(
            string password,
            string storedHash,
            string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256);

            byte[] hashBytes = pbkdf2.GetBytes(HashSize);

            string newHash = Convert.ToBase64String(hashBytes);

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(storedHash),
                Convert.FromBase64String(newHash)
            );
        }

        public static string GenerateSalt(int size = 16)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Sinh mật khẩu ngẫu nhiên gồm chữ hoa, chữ thường và chữ số sử dụng bộ sinh số ngẫu nhiên bảo mật
        /// </summary>
        public static string GenerateRandomPassword(int length = 8)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Độ dài mật khẩu phải lớn hơn 0.", nameof(length));
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new char[length];
            
            for (int i = 0; i < length; i++)
            {
                int index = RandomNumberGenerator.GetInt32(chars.Length);
                result[i] = chars[index];
            }

            return new string(result);
        }
    }
}
