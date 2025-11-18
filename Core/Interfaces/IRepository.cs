using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        Task<TEntity?> GetFirstAsync(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TResult?> GetFirstAsync<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector);

        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> predicate,
            int? pageIndex = null,
            int? pageSize = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool? asc = null,
            params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TResult>> GetAllAsync<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector,
            int? pageIndex = null,
            int? pageSize = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool? asc = null);

        Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(
            Expression<Func<TEntity, bool>>? predicate = null);

        Task AddAsync(params TEntity[] entities);
        Task UpdateAsync(params TEntity[] entities);
        Task DeleteAsync(params TEntity[] entities);
    }
}
