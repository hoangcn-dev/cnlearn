using HoangCN.Common.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ZLearn.Application.Common.Utils;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HoangCN.Common.Utils
{
    public class ConfigCommonOptions
    {
        public List<string> CorOrigins { get; set; } = [];
        public bool DisableAutoValidate { get; set; } = true;
        public bool UseGlobalExceptionCatching { get; set; } = true;
        public bool UseJwt { get; set; } = true;
    }

    public static class ProgramUtil
    {
        public static IHostApplicationBuilder ConfigCommon(this WebApplicationBuilder builder, ConfigCommonOptions? config = null)
        {
            config ??= new ConfigCommonOptions();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            if (config.UseGlobalExceptionCatching)
            {
                builder.Services.AddCatchExceptionMiddleware();
            }

            if (config.DisableAutoValidate)
            {
                builder.Services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });
            }

            if (config.CorOrigins.Count > 0)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(policy =>
                    {
                        policy
                            .WithOrigins([.. config.CorOrigins])
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });
            }

            if (config.UseJwt)
            {
                builder.Services.AddExtractCookieMiddleware();
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration.GetSection("JwtConfig")["Audience"],

                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration.GetSection("JwtConfig")["Issuer"],

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvUtil.GetValue(EnvVariableNames.JWT_SECRET_KEY)!))
                    };

                    option.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/hubs"))
                            {
                                var accessToken = context.Request.Cookies["AccessToken"];
                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    context.Token = accessToken;
                                }
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.Lax;
                })
                //.AddGoogle("Google", options =>
                //{
                //    options.ClientId = EnvHelper.GetGoogleClientId();
                //    options.ClientSecret = EnvHelper.GetGoogleClientSecret();
                //    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //    options.ClaimActions.MapJsonKey("image", "picture");
                //    options.CallbackPath = "/api/google-login";
                //})
                ;
            }

            return builder;
        }
    
        public static WebApplication BuildCommon(this WebApplicationBuilder builder,  ConfigCommonOptions? config = null)
        {
            config ??= new ConfigCommonOptions();

            var app = builder.Build();

            //var uploadsPhysicalPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
            //Directory.CreateDirectory(uploadsPhysicalPath);

            if (config.CorOrigins.Count > 0)
            {
                app.UseCors();
            }

            if (config.UseGlobalExceptionCatching)
            {
                app.UseMiddleware<CatchExceptionMiddleware>();
            }

            if (config.UseJwt)
            {
                app.UseExtractCookieMiddleware();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(uploadsPhysicalPath),
            //    RequestPath = "/uploads",
            //});
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
