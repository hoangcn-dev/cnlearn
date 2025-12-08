using Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Users.Services;
using Module.Users.Settings;

namespace Module.Users
{
    public static class DI
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.AddSingleton<JwtService>();
            services.AddTransient<IAuthService, AuthService>();

            // Google
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtService.GetApiValidationTokenParams(configuration);
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
            })
            .AddGoogle("Google", options =>
            {
                options.CallbackPath = "/google-signin";
                options.ClaimActions.MapJsonKey("image", "picture");
                options.ClientId = EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_GOOGLE_CLIENT_ID);
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClientSecret = EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_GOOGLE_CLIENT_SECRET);
            });

            return services;
        }


    }
}
