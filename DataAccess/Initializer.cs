using Core.Exceptions;
using Core.Interfaces;
using Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Users.Entities;

namespace DataAccess
{
    public class Initializer
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Initializer> _logger;

        public Initializer(
            AppDbContext context,
            ILogger<Initializer> logger,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task CheckAndUpdateFromMigrationAsync()
        {
            try
            {
                _logger.LogInformation("Start migrating...");
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while migrating the database: " + ex.Message);
                throw;
            }
        }

        public async Task CheckAndInitDefaultDataAsync()
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var roleRepo = _unitOfWork.Repository<Role>();
                var adminRole = await roleRepo.GetFirstAsync(r => r.Name.Equals(Roles.ADMIN));
                if (adminRole is null)
                {
                    adminRole = new Role
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow,
                        Name = Roles.ADMIN
                    };
                    await roleRepo.AddAsync(adminRole);
                }

                var userRole = await roleRepo.GetFirstAsync(r => r.Name.Equals(Roles.USER));
                if (userRole is null)
                {
                    userRole = new Role
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow,
                        Name = Roles.USER
                    };
                    await roleRepo.AddAsync(userRole);
                }

                var userRepo = _unitOfWork.Repository<User>();
                if (await userRepo.CountAsync() == 0)
                {
                    await userRepo.AddAsync(new User
                    {
                        Id = Guid.NewGuid(),
                        UpdatedAt = DateTimeOffset.UtcNow,
                        AvatarUrl = "https://res.cloudinary.com/dvk5yt0oi/image/upload/v1758957778/zlearn/images/hhtcmdxecquxqnsor3ip.jpg",
                        FirstName = "Nguyên",
                        LastName = "Hoàng Cao",
                        CreatedAt = DateTimeOffset.UtcNow,
                        LastLogin = DateTimeOffset.UtcNow,
                        Email = EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_ADMIN_EMAIL),
                        Password = PasswordHasher.HashPassword(EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_ADMIN_PASS)),
                        IsActived = true,
                        Role = adminRole,
                        Note = null,
                        PhoneNumber = null,
                    });
                }

                await _unitOfWork.CommitTransactionAsync();
            } 
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError("An error occurred while initializing the database: " + ex.Message);
                throw;
            }
        }
    }
}
