using Microsoft.Extensions.Logging;
using Utilities;

namespace DataAccess
{
    public class Initializer
    {
        private readonly AppDbContext _context;
        private readonly ILogger<Initializer> _logger;

        public Initializer(
            AppDbContext context,
            ILogger<Initializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UpdateDbStructÂsync()
        {
            try
            {
                _logger.LogInformation("Start migrating...");
                //await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            //await using var transaction = await _context.Database.BeginTransactionAsync();
            //try
            //{
            //    // Khởi tạo role mặc định
            //    string[] defaultRoles = new[] { "admin" };
            //    foreach (string role in defaultRoles)
            //    {
            //        if (!await _roleManager.RoleExistsAsync(role))
            //        {
            //            await _roleManager.CreateAsync(new Role
            //            {
            //                Name = role,
            //                Id = Guid.NewGuid()
            //            });
            //        }
            //    }

            //    // Khởi tạo admin
            //    if (!_userManager.Users.Any(u => u.UserName == "sysadmin"))
            //    {
            //        await _userManager.CreateAsync(new User
            //        {
            //            Id = Guid.NewGuid(),
            //            UserName = "sysadmin",
            //            Email = "hoangcn.dev@gmail.com",
            //        }, StringUtils.GetEnv(StringConsts.ENV_ADMIN_PASS));
            //    }

            //    await transaction.CommitAsync();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "An error occurred while seeding the database.");
            //    await transaction.RollbackAsync();
            //    throw;
            //}
        
        }
    }
}
