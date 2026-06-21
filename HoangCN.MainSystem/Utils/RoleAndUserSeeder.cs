using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Utils;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Interfaces;

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
                var roleBL = scope.ServiceProvider.GetRequiredService<IBaseBL<Role>>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                // 1. Seed Roles nếu chưa tồn tại
                var adminRoles = await roleBL.GetByCondition<Role>(r => r.RoleName == nameof(RoleNames.Admin));
                var userRoles = await roleBL.GetByCondition<Role>(r => r.RoleName == nameof(RoleNames.User));

                var adminRole = adminRoles.FirstOrDefault();
                var userRole = userRoles.FirstOrDefault();

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
                    await roleBL.InsertEntities(new List<Role> { adminRole });
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
                    await roleBL.InsertEntities(new List<Role> { userRole });
                }

                // 2. Seed Admin User nếu chưa tồn tại
                var users = await userService.GetByCondition<User>(u => u.UserName == "admin" || u.Email == "admin@hoangcn.com");
                var hasAdmin = users.Count > 0;
                
                if (!hasAdmin)
                {
                    var adminPassword = EnvUtil.GetValue(EnvKeys.DEFAULT_ADMIN_PASSWORD);
                    var adminUser = new User
                    {
                        UserId = Guid.NewGuid(),
                        UserName = "admin",
                        UserCode = "ADMIN001",
                        DisplayName = "Administrator",
                        Email = "admin@hoangcn.com",
                        Password = adminPassword,
                        RoleId = adminRole.RoleId,
                        IsActive = true,
                        IsVerified = true,
                        LastLogin = DateTime.UtcNow,
                        CreatedBy = "System",
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await userService.InsertEntities(new List<User> { adminUser });
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
