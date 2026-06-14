using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.DL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HoangCN.Core.DL.Implementation
{
    /// <summary>
    /// Triển khai cơ chế ghi dữ liệu đảm bảo tính toàn vẹn (ACID) sử dụng EF Core
    /// </summary>
    public class BaseWriteDL : IBaseWriteDL
    {
        private readonly DbContext _context;
        private IDbContextTransaction? _transaction;

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
        }

        /// <summary>
        /// Xác nhận lưu các thay đổi và kết thúc Transaction thành công
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
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
        }

        /// <summary>
        /// Lưu các thay đổi hiện tại vào database ghi
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lưu danh sách entity dựa trên trạng thái State của từng đối tượng
        /// </summary>
        public async Task SaveEntitiesAsync<TEntity>(List<TEntity> entities) where TEntity : BaseEntity
        {
            if (entities == null || entities.Count == 0) return;

            foreach (var entity in entities)
            {
                switch (entity.State)
                {
                    case ModelState.Insert:
                        _context.Entry(entity).State = EntityState.Added;
                        break;
                    case ModelState.Update:
                        _context.Entry(entity).State = EntityState.Modified;
                        _context.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        _context.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                        break;
                    case ModelState.Delete:
                        // Nếu đã được đánh dấu là xóa mềm, ta cập nhật Entity ở trạng thái Modified
                        if (entity.IsDeleted)
                        {
                            _context.Entry(entity).State = EntityState.Modified;
                            _context.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                            _context.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                        }
                        else
                        {
                            _context.Entry(entity).State = EntityState.Deleted;
                        }
                        break;
                }
            }

            await SaveChangesAsync();
        }
    }
}

