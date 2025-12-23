namespace Core.Utilities
{
    public class EnvUtil
    {
        public class Keys
        {
            public const string HOANGCN_PGDB_CONNECTION_STRING = nameof(HOANGCN_PGDB_CONNECTION_STRING);
            public const string HOANGCN_PGDB_FOR_HANGFIRE_CONNECTION_STRING = nameof(HOANGCN_PGDB_FOR_HANGFIRE_CONNECTION_STRING);
            public const string HOANGCN_SECRET_KEY = nameof(HOANGCN_SECRET_KEY);
            public const string HOANGCN_ADMIN_EMAIL = nameof(HOANGCN_ADMIN_EMAIL);
            public const string HOANGCN_ADMIN_PASS = nameof(HOANGCN_ADMIN_PASS);
            public const string HOANGCN_GOOGLE_CLIENT_ID = nameof(HOANGCN_GOOGLE_CLIENT_ID);
            public const string HOANGCN_GOOGLE_CLIENT_SECRET = nameof(HOANGCN_GOOGLE_CLIENT_SECRET);
            public const string HOANGCN_CLOUDINARY_NAME = nameof(HOANGCN_CLOUDINARY_NAME);
            public const string HOANGCN_CLOUDINARY_API_KEY = nameof(HOANGCN_CLOUDINARY_API_KEY);
            public const string HOANGCN_CLOUDINARY_API_SECRET = nameof(HOANGCN_CLOUDINARY_API_SECRET);
        }

        public static string GetEnv(string key)
        {
            return Environment.GetEnvironmentVariable(key)
                ?? throw new InvalidOperationException($"Required environment variable '{key}' was not found.");
        }
    }
}
