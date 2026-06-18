#nullable enable
using HoangCN.Core.Common.Base;
using HoangCN.Core.DL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace HoangCN.Core.DL.Implementation
{
    /// <summary>
    /// Triển khai cơ chế ghi dữ liệu đảm bảo tính toàn vẹn (ACID) sử dụng EF Core
    /// </summary>
    public class BaseWriteDL : IBaseWriteDL
    {
        private readonly DbContext _context;
        private IDbContextTransaction? _transaction;
        private int _transactionCount = 0;

        public BaseWriteDL(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Lấy IQueryable để thực hiện truy vấn với EF Core (hỗ trợ LINQ, Include)
        /// </summary>
        public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        /// <summary>
        /// Bắt đầu một Transaction mới
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
            _transactionCount++;
        }

        /// <summary>
        /// Xác nhận lưu các thay đổi và kết thúc Transaction thành công
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (_transactionCount > 0)
            {
                _transactionCount--;
                if (_transactionCount == 0 && _transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Hủy bỏ toàn bộ thay đổi trong Transaction khi xảy ra lỗi
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            _transactionCount = 0;
        }

        /// <summary>
        /// Lưu các thay đổi hiện tại vào database ghi
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Thêm danh sách entity vào database
        /// </summary>
        public async Task InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            if (entities == null || !entities.Any()) return;
            await _context.Set<TEntity>().AddRangeAsync(entities);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Cập nhật danh sách entity trong database
        /// </summary>
        public async Task UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            if (entities == null || !entities.Any()) return;
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
                _context.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                _context.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
            }
            await SaveChangesAsync();
        }

        /// <summary>
        /// Xóa danh sách entity (hỗ trợ cả xóa mềm/xóa cứng tùy thuộc vào IsDeleted)
        /// </summary>
        public async Task DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            if (entities == null || !entities.Any()) return;
            foreach (var entity in entities)
            {
                if (entity.IsDeleted)
                {
                    _context.Entry(entity).State = EntityState.Modified;
                    _context.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    _context.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }
                else
                {
                    _context.Set<TEntity>().Remove(entity);
                }
            }
            await SaveChangesAsync();
        }


        public void SetChanged<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertySelector, bool isChanged) where TEntity : BaseEntity
        {
            _context.Entry(entity).Property(propertySelector).IsModified = isChanged;
        }
    }
}

