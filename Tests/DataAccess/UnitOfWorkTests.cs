using Core.Interfaces;
using DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.DataAccess
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly SqliteConnection _connection;

        public UnitOfWorkTests()
        {
            // 1. Tạo SQLite in-memory connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // 2. Tạo DbContext options dùng SQLite
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(_connection)
                .Options;

            // 3. Khởi tạo DbContext và UnitOfWork
            _context = new TestDbContext(options);

            // 4. Đảm bảo database được tạo
            _context.Database.EnsureCreated();

            _unitOfWork = new UnitOfWork(_context);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
            _context.Dispose();
            _connection.Dispose();
        }

        #region Repository Tests

        [Fact]
        public void Repository_ShouldReturnSameInstanceForSameEntityType()
        {
            // Act
            var repo1 = _unitOfWork.Repository<TestEntity>();
            var repo2 = _unitOfWork.Repository<TestEntity>();

            // Assert
            Assert.NotNull(repo1);
            Assert.NotNull(repo2);
            Assert.Same(repo1, repo2);
        }

        [Fact]
        public void Repository_ShouldReturnCorrectRepositoryType()
        {
            // Act
            var repository = _unitOfWork.Repository<TestEntity>();

            // Assert
            Assert.IsAssignableFrom<IRepository<TestEntity>>(repository);
            Assert.IsType<Repository<TestEntity>>(repository);
        }

        #endregion

        #region SaveChangesAsync Tests

        [Fact]
        public async Task SaveChangesAsync_ShouldReturnNumberOfChanges()
        {
            // Arrange
            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Entity",
                Value = 100,
                CreatedAt = DateTime.Now
            };

            var repository = _unitOfWork.Repository<TestEntity>();
            await repository.AddAsync(entity);

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
        {
            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region Transaction Tests

        [Fact]
        public async Task BeginTransactionAsync_ShouldStartTransaction()
        {
            // Act & Assert - Should not throw
            await _unitOfWork.BeginTransactionAsync();
            
            // Verify transaction is active by checking we can perform operations
            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Transaction Test",
                Value = 200,
                CreatedAt = DateTime.Now
            };

            var repository = _unitOfWork.Repository<TestEntity>();
            await repository.AddAsync(entity);
            
            var result = await _unitOfWork.SaveChangesAsync();
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CommitTransactionAsync_ShouldCommitSuccessfully()
        {
            // Arrange
            await _unitOfWork.BeginTransactionAsync();
            
            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Commit Test",
                Value = 300,
                CreatedAt = DateTime.Now
            };

            var repository = _unitOfWork.Repository<TestEntity>();
            await repository.AddAsync(entity);

            // Act
            await _unitOfWork.CommitTransactionAsync();

            // Assert
            var savedEntity = await _context.TestEntities
                .FirstOrDefaultAsync(e => e.Id == entity.Id);
            Assert.NotNull(savedEntity);
            Assert.Equal("Commit Test", savedEntity.Name);
        }

        [Fact]
        public async Task CommitTransactionAsync_WithoutTransaction_ShouldOnlySaveChanges()
        {
            // Arrange
            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "No Transaction Test",
                Value = 400,
                CreatedAt = DateTime.Now
            };

            var repository = _unitOfWork.Repository<TestEntity>();
            await repository.AddAsync(entity);

            // Act
            await _unitOfWork.CommitTransactionAsync();

            // Assert
            var savedEntity = await _context.TestEntities
                .FirstOrDefaultAsync(e => e.Id == entity.Id);
            Assert.NotNull(savedEntity);
        }

        [Fact]
        public async Task RollbackTransactionAsync_ShouldRollbackChanges()
        {
            // Arrange
            await _unitOfWork.BeginTransactionAsync();
            
            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Rollback Test",
                Value = 500,
                CreatedAt = DateTime.Now
            };

            var repository = _unitOfWork.Repository<TestEntity>();
            await repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Act
            await _unitOfWork.RollbackTransactionAsync();

            // Assert
            var savedEntity = await _context.TestEntities
                .FirstOrDefaultAsync(e => e.Id == entity.Id);
            Assert.Null(savedEntity);
        }

        [Fact]
        public async Task RollbackTransactionAsync_WithoutTransaction_ShouldNotThrow()
        {
            // Act & Assert - Should not throw
            await _unitOfWork.RollbackTransactionAsync();
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task CommitTransactionAsync_WhenSaveChangesFails_ShouldRollbackAndThrow()
        {
            // Arrange
            await _unitOfWork.BeginTransactionAsync();
            
            // Create an entity that will cause a constraint violation (duplicate key)
            var entityId = Guid.NewGuid();
            var entity1 = new TestEntity
            {
                Id = entityId,
                Name = "Duplicate Test",
                Value = 600,
                CreatedAt = DateTime.Now
            };
            var entity2 = new TestEntity
            {
                Id = entityId, // Same ID to cause constraint violation
                Name = "Duplicate Test 2",
                Value = 700,
                CreatedAt = DateTime.Now
            };

            var repository = _unitOfWork.Repository<TestEntity>();
            await repository.AddAsync(entity1, entity2);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _unitOfWork.CommitTransactionAsync());

            // Verify rollback occurred
            var savedEntity = await _context.TestEntities
                .FirstOrDefaultAsync(e => e.Id == entityId);
            Assert.Null(savedEntity);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task CompleteWorkflow_WithTransaction_ShouldWorkCorrectly()
        {
            // Arrange
            var entities = new[]
            {
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1", Value = 100, CreatedAt = DateTime.Now },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2", Value = 200, CreatedAt = DateTime.Now },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 3", Value = 300, CreatedAt = DateTime.Now }
            };

            // Act
            await _unitOfWork.BeginTransactionAsync();
            
            var repository = _unitOfWork.Repository<TestEntity>();
            await repository.AddAsync(entities);
            
            await _unitOfWork.CommitTransactionAsync();

            // Assert
            var savedEntities = await _context.TestEntities.ToListAsync();
            Assert.Equal(3, savedEntities.Count);
            Assert.Contains(savedEntities, e => e.Name == "Entity 1");
            Assert.Contains(savedEntities, e => e.Name == "Entity 2");
            Assert.Contains(savedEntities, e => e.Name == "Entity 3");
        }

        [Fact]
        public async Task MultipleRepositories_ShouldWorkWithSameUnitOfWork()
        {
            // Note: This test demonstrates the pattern even though we only have one entity type
            // In a real scenario, you would have multiple entity types
            
            // Arrange
            var entity1 = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Multi Repo Test 1",
                Value = 100,
                CreatedAt = DateTime.Now
            };

            // Act
            var repo1 = _unitOfWork.Repository<TestEntity>();
            var repo2 = _unitOfWork.Repository<TestEntity>(); // Should return same instance
            
            await repo1.AddAsync(entity1);
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            Assert.Same(repo1, repo2);
            Assert.Equal(1, result);
            
            var savedEntity = await _context.TestEntities
                .FirstOrDefaultAsync(e => e.Id == entity1.Id);
            Assert.NotNull(savedEntity);
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_ShouldCleanupResources()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_context);

            // Act
            unitOfWork.Dispose();

            // Assert - Should not throw when calling dispose multiple times
            unitOfWork.Dispose();
        }

        [Fact]
        public async Task Dispose_WithActiveTransaction_ShouldCleanupTransaction()
        {
            // Arrange - Use SQLite instead of in-memory database
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;
            
            var context = new TestDbContext(options);
            context.Database.EnsureCreated();
            
            var unitOfWork = new UnitOfWork(context);

            await unitOfWork.BeginTransactionAsync();

            // Act
            unitOfWork.Dispose();
            
            // Cleanup
            context.Dispose();
            connection.Dispose();

            // Assert - Should not throw
            Assert.True(true); // If we reach here, dispose worked correctly
        }

        #endregion
    }
}
