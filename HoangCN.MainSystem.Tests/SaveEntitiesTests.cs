using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
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
    #region Test Entities

    public class TestParent : BaseEntity
    {
        [Key]
        public Guid TestParentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual List<TestChild> Children { get; set; } = new();
    }

    public class TestChild : BaseEntity
    {
        [Key]
        public Guid TestChildId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public Guid TestParentId { get; set; }

        [ForeignKey(nameof(TestParentId))]
        public virtual TestParent? TestParent { get; set; }
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TestParent> TestParents { get; set; }
        public DbSet<TestChild> TestChildren { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestParent>().ToTable("TestParent");
            modelBuilder.Entity<TestChild>().ToTable("TestChild");
        }
    }

    #endregion

    public class SaveEntitiesTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TestDbContext _context;
        private readonly BaseWriteDL _writeDL;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;

        public SaveEntitiesTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new TestDbContext(options);
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

        [Fact]
        public async Task SaveEntities_Should_InsertParentAndChildren_When_HierarchyIsNew()
        {
            // Arrange
            var parent = new TestParent
            {
                Name = "Parent 1",
                Children = new List<TestChild>
                {
                    new TestChild { Name = "Child 1.1" },
                    new TestChild { Name = "Child 1.2" }
                }
            };

            // Act
            var result = await _writeDL.SaveEntities(new List<TestParent> { parent });

            // Assert
            Assert.NotEmpty(result);
            var savedParent = await _context.TestParents.Include(p => p.Children).FirstOrDefaultAsync();
            Assert.NotNull(savedParent);
            Assert.NotEqual(Guid.Empty, savedParent.TestParentId);
            Assert.Equal("Parent 1", savedParent.Name);
            Assert.Equal(2, savedParent.Children.Count);
            Assert.All(savedParent.Children, child => {
                Assert.NotEqual(Guid.Empty, child.TestChildId);
                Assert.Equal(savedParent.TestParentId, child.TestParentId);
                Assert.Equal(ModalState.Insert, child.State);
            });
        }

        [Fact]
        public async Task SaveEntities_Should_SynchronizeChildren_When_ParentIsUpdated()
        {
            // Arrange: Tạo dữ liệu ban đầu trong DB
            var parent = new TestParent
            {
                TestParentId = Guid.NewGuid(),
                Name = "Parent Original",
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };
            var child1 = new TestChild
            {
                TestChildId = Guid.NewGuid(),
                TestParentId = parent.TestParentId,
                Name = "Child Keep",
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };
            var child2 = new TestChild
            {
                TestChildId = Guid.NewGuid(),
                TestParentId = parent.TestParentId,
                Name = "Child Delete",
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            await _context.TestParents.AddAsync(parent);
            await _context.TestChildren.AddRangeAsync(child1, child2);
            await _context.SaveChangesAsync();

            // Clear tracker của EF để tránh conflict dữ liệu cũ
            _context.ChangeTracker.Clear();

            // Client cập nhật:
            // - Đổi tên parent
            // - Đổi tên child1 (Keep)
            // - Thêm con mới child3
            // - Không truyền child2 lên (Delete/Orphan)
            var parentPayload = new TestParent
            {
                TestParentId = parent.TestParentId,
                Name = "Parent Updated",
                Children = new List<TestChild>
                {
                    new TestChild { TestChildId = child1.TestChildId, Name = "Child Keep Updated" },
                    new TestChild { Name = "Child New" }
                }
            };

            // Act
            var result = await _writeDL.SaveEntities(new List<TestParent> { parentPayload });

            // Assert
            var savedParent = await _context.TestParents.Include(p => p.Children).FirstOrDefaultAsync();
            Assert.NotNull(savedParent);
            Assert.Equal("Parent Updated", savedParent.Name);

            // Kiểm tra đồng bộ con
            Assert.Equal(2, savedParent.Children.Count);
            Assert.Contains(savedParent.Children, c => c.Name == "Child Keep Updated");
            Assert.Contains(savedParent.Children, c => c.Name == "Child New");
            Assert.DoesNotContain(savedParent.Children, c => c.Name == "Child Delete");

            // Kiểm tra trong DB xem child2 đã bị xóa cứng/mềm chưa
            var childDeleteInDb = await _context.TestChildren.FindAsync(child2.TestChildId);
            Assert.Null(childDeleteInDb); // Bị xóa cứng
        }

        [Fact]
        public async Task SaveEntities_Should_AutoAssignFK_When_ParentIsNew()
        {
            // Arrange
            var parent = new TestParent
            {
                Name = "Parent New",
                Children = new List<TestChild>
                {
                    new TestChild { Name = "Child" }
                }
            };

            // Act
            await _writeDL.SaveEntities(new List<TestParent> { parent });

            // Assert
            var savedParent = await _context.TestParents.Include(p => p.Children).FirstOrDefaultAsync();
            Assert.NotNull(savedParent);
            var child = savedParent.Children.FirstOrDefault();
            Assert.NotNull(child);
            Assert.Equal(savedParent.TestParentId, child.TestParentId);
        }

        [Fact]
        public async Task SaveEntities_Should_RollbackTransaction_When_ErrorOccurs()
        {
            // Arrange: Tạo dữ liệu với con bị lỗi (Name là null gây vi phạm NOT NULL constraint trong SQLite)
            var parent = new TestParent
            {
                Name = "Parent Unique",
                Children = new List<TestChild>
                {
                    new TestChild { Name = null! } // Lỗi
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _writeDL.SaveEntities(new List<TestParent> { parent });
            });

            // Kiểm tra DB xem parent có bị ghi không (nên rỗng do Rollback)
            var savedParent = await _context.TestParents.FirstOrDefaultAsync();
            Assert.Null(savedParent);
        }

        [Fact]
        public async Task SaveEntities_Should_NotOverwriteCreatedByAndCreatedDate_When_EntitiesAreUpdated()
        {
            // Arrange: Thêm sẵn cha và con với CreatedBy = "System"
            var parentId = Guid.NewGuid();
            var childId = Guid.NewGuid();
            var originalTime = DateTime.UtcNow.AddHours(-1); // Đặt thời gian trước 1 tiếng

            var parent = new TestParent
            {
                TestParentId = parentId,
                Name = "Parent Original",
                CreatedBy = "System",
                CreatedDate = originalTime,
                ModifiedDate = originalTime
            };
            var child = new TestChild
            {
                TestChildId = childId,
                TestParentId = parentId,
                Name = "Child Original",
                CreatedBy = "System",
                CreatedDate = originalTime,
                ModifiedDate = originalTime
            };

            await _context.TestParents.AddAsync(parent);
            await _context.TestChildren.AddAsync(child);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Client cập nhật: Gửi payload lên với CreatedBy = null, CreatedDate = mặc định
            var parentPayload = new TestParent
            {
                TestParentId = parentId,
                Name = "Parent Updated Name",
                CreatedBy = null!,      // Client gửi null
                CreatedDate = default,  // Client gửi default
                Children = new List<TestChild>
                {
                    new TestChild 
                    { 
                        TestChildId = childId, 
                        Name = "Child Updated Name", 
                        CreatedBy = null!,      // Client gửi null
                        CreatedDate = default   // Client gửi default
                    }
                }
            };

            // Act: Gọi SaveEntities
            await _writeDL.SaveEntities(new List<TestParent> { parentPayload });

            // Assert: Kiểm tra DB xem CreatedBy và CreatedDate ban đầu có được giữ nguyên không
            var savedParentInDb = await _context.TestParents.FindAsync(parentId);
            Assert.NotNull(savedParentInDb);
            Assert.Equal("System", savedParentInDb.CreatedBy); // Vẫn là System
            Assert.Equal(originalTime.ToString("yyyy-MM-dd HH:mm"), savedParentInDb.CreatedDate.ToString("yyyy-MM-dd HH:mm")); // Vẫn giữ nguyên giờ cũ
            Assert.Equal("Parent Updated Name", savedParentInDb.Name);

            var savedChild = await _context.TestChildren.FindAsync(childId);
            Assert.NotNull(savedChild);
            Assert.Equal("System", savedChild.CreatedBy); // Vẫn là System
            Assert.Equal(originalTime.ToString("yyyy-MM-dd HH:mm"), savedChild.CreatedDate.ToString("yyyy-MM-dd HH:mm")); // Vẫn giữ nguyên giờ cũ
            Assert.Equal("Child Updated Name", savedChild.Name);
        }
    }
}
