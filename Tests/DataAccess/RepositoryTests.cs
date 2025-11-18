using Core.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Xunit;

namespace Tests.DataAccess
{
    // Test entity for testing purposes
    public class TestEntity : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    // Test DbContext
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; }
    }

    public class RepositoryTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly Repository<TestEntity> _repository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TestDbContext(options);
            _repository = new Repository<TestEntity>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task SeedTestDataAsync()
        {
            var entities = new[]
            {
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1", Value = 10, CreatedAt = DateTime.Now.AddDays(-3) },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2", Value = 20, CreatedAt = DateTime.Now.AddDays(-2) },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 3", Value = 30, CreatedAt = DateTime.Now.AddDays(-1) },
                new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity", Value = 40, CreatedAt = DateTime.Now }
            };

            await _context.TestEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddAsync_ShouldAddSingleEntity()
        {
            // Arrange
            var entity = new TestEntity 
            { 
                Id = Guid.NewGuid(), 
                Name = "New Entity", 
                Value = 100,
                CreatedAt = DateTime.Now
            };

            // Act
            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Assert
            var addedEntity = await _context.TestEntities.FirstOrDefaultAsync(e => e.Id == entity.Id);
            Assert.NotNull(addedEntity);
            Assert.Equal("New Entity", addedEntity.Name);
            Assert.Equal(100, addedEntity.Value);
        }

        [Fact]
        public async Task AddAsync_ShouldAddMultipleEntities()
        {
            // Arrange
            var entities = new[]
            {
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity A", Value = 1, CreatedAt = DateTime.Now },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity B", Value = 2, CreatedAt = DateTime.Now }
            };

            // Act
            await _repository.AddAsync(entities);
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.TestEntities.CountAsync();
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task CountAsync_WithoutPredicate_ShouldReturnTotalCount()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var count = await _repository.CountAsync();

            // Assert
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var count = await _repository.CountAsync(e => e.Value > 20);

            // Assert
            Assert.Equal(2, count); // Entity 3 (30) and Test Entity (40)
        }

        [Fact]
        public async Task ExistsAsync_WithExistingEntity_ShouldReturnTrue()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var exists = await _repository.ExistsAsync(e => e.Name == "Entity 1");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingEntity_ShouldReturnFalse()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var exists = await _repository.ExistsAsync(e => e.Name == "Non-existing Entity");

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task GetFirstAsync_WithoutSelector_ShouldReturnEntity()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var entity = await _repository.GetFirstAsync(e => e.Name == "Entity 1");

            // Assert
            Assert.NotNull(entity);
            Assert.Equal("Entity 1", entity.Name);
            Assert.Equal(10, entity.Value);
        }

        [Fact]
        public async Task GetFirstAsync_WithSelector_ShouldReturnMappedResult()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var name = await _repository.GetFirstAsync(
                e => e.Value == 20,
                e => e.Name);

            // Assert
            Assert.Equal("Entity 2", name);
        }

        [Fact]
        public async Task GetFirstAsync_WithNonExistingEntity_ShouldReturnNull()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var entity = await _repository.GetFirstAsync(e => e.Name == "Non-existing");

            // Assert
            Assert.Null(entity);
        }

        [Fact]
        public async Task GetAllAsync_WithoutSelector_ShouldReturnAllMatchingEntities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var entities = await _repository.GetAllAsync(e => e.Value >= 20);

            // Assert
            Assert.Equal(3, entities.Count()); // Entity 2 (20), Entity 3 (30), Test Entity (40)
        }

        [Fact]
        public async Task GetAllAsync_WithSelector_ShouldReturnMappedResults()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var names = await _repository.GetAllAsync(
                e => e.Value >= 20,
                e => e.Name);

            // Assert
            var namesList = names.ToList();
            Assert.Equal(3, namesList.Count);
            Assert.Contains("Entity 2", namesList);
            Assert.Contains("Entity 3", namesList);
            Assert.Contains("Test Entity", namesList);
        }

        [Fact]
        public async Task GetAllAsync_WithPagination_ShouldReturnPagedResults()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var entities = await _repository.GetAllAsync(
                e => true,
                pageIndex: 1,
                pageSize: 2,
                orderBy: e => e.Value,
                asc: true);

            // Assert
            var entitiesList = entities.ToList();
            Assert.Equal(2, entitiesList.Count);
        }

        [Fact]
        public async Task GetAllAsync_WithOrderingAscending_ShouldReturnOrderedResults()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var entities = await _repository.GetAllAsync(
                e => true,
                orderBy: e => e.Value,
                asc: true);

            // Assert
            var entitiesList = entities.Cast<TestEntity>().ToList();
            Assert.Equal(10, entitiesList[0].Value);
            Assert.Equal(20, entitiesList[1].Value);
            Assert.Equal(30, entitiesList[2].Value);
            Assert.Equal(40, entitiesList[3].Value);
        }

        [Fact]
        public async Task GetAllAsync_WithOrderingDescending_ShouldReturnOrderedResults()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var entities = await _repository.GetAllAsync(
                e => true,
                orderBy: e => e.Value,
                asc: false);

            // Assert
            var entitiesList = entities.Cast<TestEntity>().ToList();
            Assert.Equal(40, entitiesList[0].Value);
            Assert.Equal(30, entitiesList[1].Value);
            Assert.Equal(20, entitiesList[2].Value);
            Assert.Equal(10, entitiesList[3].Value);
        }

        [Fact]
        public async Task UpdateAsync_ShouldMarkEntityAsModified()
        {
            // Arrange
            await SeedTestDataAsync();
            var entity = await _context.TestEntities.FirstAsync(e => e.Name == "Entity 1");
            entity.Name = "Updated Entity 1";
            entity.Value = 999;

            // Act
            await _repository.UpdateAsync(entity);
            await _context.SaveChangesAsync();

            // Assert
            var updatedEntity = await _context.TestEntities.FirstAsync(e => e.Id == entity.Id);
            Assert.Equal("Updated Entity 1", updatedEntity.Name);
            Assert.Equal(999, updatedEntity.Value);
        }

        [Fact]
        public async Task UpdateAsync_WithMultipleEntities_ShouldUpdateAll()
        {
            // Arrange
            await SeedTestDataAsync();
            var entities = await _context.TestEntities.Where(e => e.Value <= 20).ToListAsync();
            foreach (var entity in entities)
            {
                entity.Value += 1000;
            }

            // Act
            await _repository.UpdateAsync(entities.ToArray());
            await _context.SaveChangesAsync();

            // Assert
            var updatedEntities = await _context.TestEntities.Where(e => e.Value >= 1000).ToListAsync();
            Assert.Equal(2, updatedEntities.Count);
            Assert.Contains(updatedEntities, e => e.Value == 1010);
            Assert.Contains(updatedEntities, e => e.Value == 1020);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            // Arrange
            await SeedTestDataAsync();
            var entity = await _context.TestEntities.FirstAsync(e => e.Name == "Entity 1");

            // Act
            await _repository.DeleteAsync(entity);
            await _context.SaveChangesAsync();

            // Assert
            var deletedEntity = await _context.TestEntities.FirstOrDefaultAsync(e => e.Id == entity.Id);
            Assert.Null(deletedEntity);
            
            var remainingCount = await _context.TestEntities.CountAsync();
            Assert.Equal(3, remainingCount);
        }

        [Fact]
        public async Task DeleteAsync_WithMultipleEntities_ShouldRemoveAll()
        {
            // Arrange
            await SeedTestDataAsync();
            var entitiesToDelete = await _context.TestEntities.Where(e => e.Value <= 20).ToListAsync();

            // Act
            await _repository.DeleteAsync(entitiesToDelete.ToArray());
            await _context.SaveChangesAsync();

            // Assert
            var remainingCount = await _context.TestEntities.CountAsync();
            Assert.Equal(2, remainingCount); // Only Entity 3 and Test Entity should remain
            
            var remainingEntities = await _context.TestEntities.ToListAsync();
            Assert.All(remainingEntities, e => Assert.True(e.Value > 20));
        }

        [Fact]
        public async Task GetAllAsync_WithComplexPaginationAndOrdering_ShouldWork()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act - Get page 2 with page size 1, ordered by Value descending
            var entities = await _repository.GetAllAsync(
                e => true,
                e => e.Name,
                pageIndex: 2,
                pageSize: 1,
                orderBy: e => e.Value,
                asc: false);

            // Assert
            var entitiesList = entities.ToList();
            Assert.Single(entitiesList);
            // Should be the second highest value (30) - Entity 3
            Assert.Equal("Entity 3", entitiesList[0]);
        }
    }
}
