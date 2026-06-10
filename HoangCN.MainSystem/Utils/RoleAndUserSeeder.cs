using HoangCN.Core.DL;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Enums;
using HoangCN.MainSystem.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HoangCN.MainSystem.Utils
{
    /// <summary>
    /// Seeder tự động nạp các vai trò (Roles) và tài khoản quản trị (Admin) mặc định vào cơ sở dữ liệu khi khởi chạy
    /// </summary>
    public class RoleAndUserSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public RoleAndUserSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DynamicDbContext>();

                // 1. Seed Roles nếu chưa tồn tại
                var roles = dbContext.Set<Role>();
                var adminRole = await roles.FirstOrDefaultAsync(r => r.RoleName == nameof(RoleNames.Admin), cancellationToken);
                var userRole = await roles.FirstOrDefaultAsync(r => r.RoleName == nameof(RoleNames.User), cancellationToken);

                if (adminRole == null)
                {
                    adminRole = new Role
                    {
                        RoleId = Guid.NewGuid(),
                        RoleName = nameof(RoleNames.Admin),
                        CreatedBy = "System",
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await roles.AddAsync(adminRole, cancellationToken);
                }

                if (userRole == null)
                {
                    userRole = new Role
                    {
                        RoleId = Guid.NewGuid(),
                        RoleName = nameof(RoleNames.User),
                        CreatedBy = "System",
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await roles.AddAsync(userRole, cancellationToken);
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                // 2. Seed Admin User nếu chưa tồn tại
                var users = dbContext.Set<User>();
                var hasAdmin = await users.AnyAsync(u => u.UserName == "admin" || u.Email == "admin@hoangcn.com", cancellationToken);
                if (!hasAdmin)
                {
                    var adminPassword = EnvKeyUtil.GetValue(EnvKeyUtil.DEFAULT_ADMIN_PASSWORD);
                    var (hash, salt) = PasswordUtil.HashPassword(adminPassword);
                    var adminUser = new User
                    {
                        UserId = Guid.NewGuid(),
                        UserName = "admin",
                        UserCode = "ADMIN001",
                        DisplayName = "Administrator",
                        Email = "admin@hoangcn.com",
                        Password = hash,
                        PasswordSalt = salt,
                        IsActive = true,
                        IsVerified = true,
                        RoleId = adminRole.RoleId,
                        LastLogin = DateTime.UtcNow,
                        CreatedBy = "System",
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await users.AddAsync(adminUser, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
