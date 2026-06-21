using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Attributes;
using HoangCN.Core.DL.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace HoangCN.MainSystem.Tests
{
    #region Test Entities for CheckExist

    public class TestParentCheck : BaseEntity
    {
        [Key]
        public Guid TestParentCheckId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class TestChildCheck : BaseEntity
    {
        [Key]
        public Guid TestChildCheckId { get; set; }

        [Required]
        [StringLength(100)]
        [CheckExist(MustExist = false, ErrorMessage = "Mã Code bị trùng lặp")]
        public string Code { get; set; } = string.Empty;

        [CheckExist(MustExist = true, TargetEntity = typeof(TestParentCheck), ErrorMessage = "Thực thể cha không tồn tại")]
        public Guid ParentId { get; set; }
    }

    public class CheckExistTestDbContext : DbContext
    {
        public CheckExistTestDbContext(DbContextOptions<CheckExistTestDbContext> options) : base(options) { }

        public DbSet<TestParentCheck> TestParentChecks { get; set; }
        public DbSet<TestChildCheck> TestChildChecks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestParentCheck>().ToTable("TestParentCheck");
            modelBuilder.Entity<TestChildCheck>().ToTable("TestChildCheck");
        }
    }

    #endregion

    public class CheckExistTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly CheckExistTestDbContext _context;
        private readonly BaseWriteDL _writeDL;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;

        public CheckExistTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<CheckExistTestDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new CheckExistTestDbContext(options);
            _context.Database.EnsureCreated();

            // Mock IHttpContextAccessor
            _httpContextMock = new Mock<IHttpContextAccessor>();
            var claims = new List<Claim> { new Claim("UserName", "TestUser") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);
            _httpContextMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _writeDL = new BaseWriteDL(_context, _httpContextMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        #region Kịch bản 1: Validate Khóa chính (Primary Key Validation)

        [Fact]
        public async Task CheckExist_ShouldThrowBadRequestException_WhenPrimaryKeyDoesNotExistOnUpdate()
        {
            // Arrange: Tạo thực thể update có ID không tồn tại trong DB
            var child = new TestChildCheck
            {
                TestChildCheckId = Guid.NewGuid(), // ID ngẫu nhiên không có trong DB
                Code = "C01",
                State = ModalState.Update
            };

            // Act & Assert: Gọi SaveEntities (sẽ tự động validate qua CheckExist ở Pha 3)
            var ex = await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _writeDL.SaveEntities(new List<TestChildCheck> { child });
            });

            Assert.Contains("không tồn tại", ex.Message);
        }

        #endregion

        #region Kịch bản 2: Validate Trùng lặp trên Danh sách (List Duplicate Validation)

        [Fact]
        public async Task CheckExist_ShouldThrowBadRequestException_WhenCodeIsDuplicatedInIncomingList()
        {
            // Arrange: Gửi lên danh sách chứa 2 thực thể cùng mã Code "C01"
            var parent = new TestParentCheck { TestParentCheckId = Guid.NewGuid(), Name = "Parent", CreatedBy = "System" };
            await _context.TestParentChecks.AddAsync(parent);
            await _context.SaveChangesAsync();

            var child1 = new TestChildCheck { Code = "C01", ParentId = parent.TestParentCheckId };
            var child2 = new TestChildCheck { Code = "C01", ParentId = parent.TestParentCheckId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _writeDL.SaveEntities(new List<TestChildCheck> { child1, child2 });
            });

            Assert.Contains("bị trùng lặp trên danh sách", ex.Message);
        }

        #endregion

        #region Kịch bản 3: Validate Trùng lặp với DB khi Thêm mới (DB Duplicate Validation on Insert)

        [Fact]
        public async Task CheckExist_ShouldThrowBadRequestException_WhenCodeAlreadyExistsInDbOnInsert()
        {
            // Arrange: Thêm sẵn 1 bản ghi có Code "C01" vào DB
            var parent = new TestParentCheck { TestParentCheckId = Guid.NewGuid(), Name = "Parent", CreatedBy = "System" };
            var existingChild = new TestChildCheck { TestChildCheckId = Guid.NewGuid(), Code = "C01", ParentId = parent.TestParentCheckId, CreatedBy = "System" };
            await _context.TestParentChecks.AddAsync(parent);
            await _context.TestChildChecks.AddAsync(existingChild);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Thêm mới thực thể khác có cùng Code "C01"
            var newChild = new TestChildCheck { Code = "C01", ParentId = parent.TestParentCheckId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _writeDL.SaveEntities(new List<TestChildCheck> { newChild });
            });

            Assert.Contains("đã tồn tại", ex.Message);
        }

        #endregion

        #region Kịch bản 4: Validate Trùng lặp với DB khi Cập nhật (DB Duplicate Validation on Update)

        [Fact]
        public async Task CheckExist_ShouldThrowBadRequestException_WhenCodeAlreadyExistsInDbOnUpdate()
        {
            // Arrange: Thêm sẵn 2 bản ghi có Code "C01" và "C02" vào DB
            var parent = new TestParentCheck { TestParentCheckId = Guid.NewGuid(), Name = "Parent", CreatedBy = "System" };
            var childA = new TestChildCheck { TestChildCheckId = Guid.NewGuid(), Code = "C01", ParentId = parent.TestParentCheckId, CreatedBy = "System" };
            var childB = new TestChildCheck { TestChildCheckId = Guid.NewGuid(), Code = "C02", ParentId = parent.TestParentCheckId, CreatedBy = "System" };
            await _context.TestParentChecks.AddAsync(parent);
            await _context.TestChildChecks.AddRangeAsync(childA, childB);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Client cập nhật bản ghi B và đổi Code thành "C01" (trùng với A)
            var updateChildB = new TestChildCheck
            {
                TestChildCheckId = childB.TestChildCheckId,
                Code = "C01",
                ParentId = parent.TestParentCheckId
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _writeDL.SaveEntities(new List<TestChildCheck> { updateChildB });
            });

            Assert.Contains("đã tồn tại", ex.Message);
        }

        #endregion

        #region Kịch bản 5: Bỏ qua trùng lặp khi Cập nhật chính mình (Self-Ignore on Update)

        [Fact]
        public async Task CheckExist_ShouldPass_WhenUpdatingSelfWithSameCode()
        {
            // Arrange: Thêm sẵn bản ghi có Code "C01"
            var parent = new TestParentCheck { TestParentCheckId = Guid.NewGuid(), Name = "Parent", CreatedBy = "System" };
            var child = new TestChildCheck { TestChildCheckId = Guid.NewGuid(), Code = "C01", ParentId = parent.TestParentCheckId, CreatedBy = "System" };
            await _context.TestParentChecks.AddAsync(parent);
            await _context.TestChildChecks.AddAsync(child);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Client cập nhật chính bản ghi đó, đổi trường Code nhưng thực tế giữ nguyên "C01"
            var updateChild = new TestChildCheck
            {
                TestChildCheckId = child.TestChildCheckId,
                Code = "C01",
                ParentId = parent.TestParentCheckId
            };

            // Act: Gọi SaveEntities
            var result = await _writeDL.SaveEntities(new List<TestChildCheck> { updateChild });

            // Assert: Không báo lỗi và lưu thành công
            Assert.NotEmpty(result);
            var savedChild = await _context.TestChildChecks.FindAsync(child.TestChildCheckId);
            Assert.NotNull(savedChild);
            Assert.Equal("C01", savedChild.Code);
        }

        #endregion

        #region Kịch bản 6: Validate Khóa ngoại tồn tại (Foreign Key Presence Validation)

        [Fact]
        public async Task CheckExist_ShouldThrowBadRequestException_WhenParentIdDoesNotExistInDb()
        {
            // Arrange: Thực thể con có ParentId trỏ tới một cha không tồn tại (Guid ngẫu nhiên)
            var child = new TestChildCheck
            {
                Code = "C01",
                ParentId = Guid.NewGuid() // ID cha giả không tồn tại
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _writeDL.SaveEntities(new List<TestChildCheck> { child });
            });

            Assert.Contains("không tồn tại", ex.Message);
        }

        [Fact]
        public async Task CheckExist_ShouldPass_WhenParentIdExistsInDb()
        {
            // Arrange: Tạo và lưu cha hợp lệ vào DB
            var parent = new TestParentCheck { TestParentCheckId = Guid.NewGuid(), Name = "Parent", CreatedBy = "System" };
            await _context.TestParentChecks.AddAsync(parent);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var child = new TestChildCheck
            {
                Code = "C01",
                ParentId = parent.TestParentCheckId
            };

            // Act: Gọi SaveEntities
            var result = await _writeDL.SaveEntities(new List<TestChildCheck> { child });

            // Assert: Lưu thành công
            Assert.NotEmpty(result);
            var savedChild = await _context.TestChildChecks.FindAsync(result[0].TestChildCheckId);
            Assert.NotNull(savedChild);
            Assert.Equal(parent.TestParentCheckId, savedChild.ParentId);
        }

        #endregion
    }
}
