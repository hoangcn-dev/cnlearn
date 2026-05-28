namespace ZLearn.Application.Common.Utils
{
    public abstract class EnvUtil
    {
        public static string GetValue(string varName)
        {
            return Environment.GetEnvironmentVariable(varName)
                ?? throw new InvalidOperationException($"Required environment variable '{varName}' was not found.");
        }
    }

    public class EnvVariableNames
    {
        public const string JWT_SECRET_KEY = nameof(JWT_SECRET_KEY);
        public const string GOOGLE_CLIENT_ID = nameof(GOOGLE_CLIENT_ID);
        public const string GOOGLE_CLIENT_SECRET = nameof(GOOGLE_CLIENT_SECRET);
        public const string CLOUDINARY_NAME = nameof(CLOUDINARY_NAME);
        public const string CLOUDINARY_API_KEY = nameof(CLOUDINARY_API_KEY);
        public const string CLOUDINARY_API_SECRET = nameof(CLOUDINARY_API_SECRET);
        public const string GROQ_API_KEY = nameof(GROQ_API_KEY);
        public const string HOANGCN_EMAIL_BOT_APP_PASSWORD = nameof(HOANGCN_EMAIL_BOT_APP_PASSWORD);
        public const string HOANGCN_EMAIL_BOT_APP_EMAIL = nameof(HOANGCN_EMAIL_BOT_APP_EMAIL);
    }

}