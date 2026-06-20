using Dapper;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Utils;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Metadata;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.Core.DL.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HoangCN.Core.BL.Base
{
    /// <summary>
    /// Lớp triển khai cho giao diện cơ sở tầng nghiệp vụ sử dụng kết hợp EF Core và Dapper
    /// </summary>
    public class BaseBL<TEntity> : IBaseBL<TEntity> where TEntity : BaseEntity
    {
        protected readonly IBaseReadDL _baseReadDL;
        protected readonly IBaseWriteDL _baseWriteDL;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseBL(
            IBaseReadDL baseReadDL,
            IBaseWriteDL baseWriteDL,
            IHttpContextAccessor httpContextAccessor)
        {
            _baseReadDL = baseReadDL ?? throw new ArgumentNullException(nameof(baseReadDL));
            _baseWriteDL = baseWriteDL ?? throw new ArgumentNullException(nameof(baseWriteDL));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Xóa nhiều đối tượng thông qua danh sách ID
        /// </summary>
        public virtual async Task DeleteAsync(DeleteRequest request)
        {
            var res = await Get<TEntity>(new GetRequest { Ids = request.Ids });
            if (res.Items.Count == 0)
            {
                return;
            }


            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                await _baseWriteDL.DeleteRangeAsync(res.Items);
                await AfterDelete(res.Items);
                await _baseWriteDL.CommitTransactionAsync();
            }
            catch
            {
                await _baseWriteDL.RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách đối tượng sử dụng Dapper (Read DB)
        /// </summary>
        public async Task<ResultDto<TResult>> Get<TResult>(GetRequest request)
        {
            var parameters = new DynamicParameters();
            var mainTableName = typeof(TEntity).Name;
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>();
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity, TResult>(request, parameters);
            var sortClause = BuildSQLUtil.BuildSortClaude<TEntity>(request);

            var result = new ResultDto<TResult>();

            if (request.IsPaging)
            {
                // 1. Tính tổng số dòng (COUNT) sử dụng Dapper
                var countSql = $"SELECT COUNT(*) {BuildSQLUtil.BuildFromClaude<TEntity, TResult>()} {whereClause}";
                var totalCount = await _baseReadDL.ExecuteQueryToGetFirstResult<int>(countSql, parameters);
                result.Total = totalCount;
                result.Page = request.Page ?? 1;
                result.Size = request.Size ?? 10;

                // 2. Lấy danh sách phần tử (PAGING) sử dụng Dapper
                var pagingSql = $"{selectFromSql} {whereClause} {sortClause}" + BuildSQLUtil.BuildPagingClaude(result.Page, result.Size, parameters);
                var items = await _baseReadDL.ExecuteQueryText<TResult>(pagingSql, parameters);
                result.Items = items.ToList();
            }
            else
            {
                var sql = $"{selectFromSql} {whereClause} {sortClause}";
                var items = await _baseReadDL.ExecuteQueryText<TResult>(sql, parameters);
                result.Items = items.ToList();
                result.Total = result.Items.Count;
                result.Page = 1;
                result.Size = result.Total > 0 ? result.Total : 1;
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách đối tượng sử dụng Dapper (Read DB) kèm điều kiện Expression
        /// </summary>
        public async Task<ResultDto<TResult>> Get<TResult>(GetRequest request, Expression<Func<TEntity, bool>> condition)
        {
            var parameters = new DynamicParameters();
            var mainTableName = typeof(TEntity).Name;
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>();
            
            var extraGroup = ExpressionToFilterTranslator.Translate(condition);
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity, TResult>(request, parameters, extraGroup);
            var sortClause = BuildSQLUtil.BuildSortClaude<TEntity>(request);

            var result = new ResultDto<TResult>();

            if (request.IsPaging)
            {
                // 1. Tính tổng số dòng (COUNT) sử dụng Dapper
                var countSql = $"SELECT COUNT(*) {BuildSQLUtil.BuildFromClaude<TEntity, TResult>()} {whereClause}";
                var totalCount = await _baseReadDL.ExecuteQueryToGetFirstResult<int>(countSql, parameters);
                result.Total = totalCount;
                result.Page = request.Page ?? 1;
                result.Size = request.Size ?? 10;

                // 2. Lấy danh sách phần tử (PAGING) sử dụng Dapper
                var pagingSql = $"{selectFromSql} {whereClause} {sortClause}" + BuildSQLUtil.BuildPagingClaude(result.Page, result.Size, parameters);
                var items = await _baseReadDL.ExecuteQueryText<TResult>(pagingSql, parameters);
                result.Items = items.ToList();
            }
            else
            {
                var sql = $"{selectFromSql} {whereClause} {sortClause}";
                var items = await _baseReadDL.ExecuteQueryText<TResult>(sql, parameters);
                result.Items = items.ToList();
                result.Total = result.Items.Count;
                result.Page = 1;
                result.Size = result.Total > 0 ? result.Total : 1;
            }

            return result;
        }

        /// <summary>
        /// Lấy chi tiết đối tượng theo ID sử dụng Dapper tối ưu hóa
        /// </summary>
        public async Task<TResult?> GetById<TResult>(Guid id)
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var pkPropName = metadata.PrimaryKeyName;

            // Xây dựng biểu thức Lambda động: entity => entity.PrimaryKey == id
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var property = Expression.Property(parameter, pkPropName);
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);

            return await GetSingleByCondition<TResult>(lambda);
        }

        /// <summary>
        /// Thêm mới danh sách thực thể
        /// </summary>
        public virtual async Task InsertAsync(List<TEntity> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                throw new BadRequestException("Dữ liệu trống hoặc sai định dạng");
            }

            await BeforeInsert(entities);
            ValidateUtil.CommonValidate(entities);
            await ValidateUtil.CheckExist(entities, _baseReadDL);

            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                await _baseWriteDL.InsertRangeAsync(entities);
                await AfterInsert(entities);
                await _baseWriteDL.CommitTransactionAsync();
            }
            catch
            {
                await _baseWriteDL.RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Cập nhật danh sách thực thể
        /// </summary>
        public virtual async Task UpdateAsync(List<TEntity> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                throw new BadRequestException("Dữ liệu trống hoặc sai định dạng");
            }

            await BeforeUpdate(entities);
            ValidateUtil.CommonValidate(entities, [nameof(BaseEntity.CreatedBy), nameof(BaseEntity.CreatedDate)]);
            await ValidateUtil.ValidatePkExists(entities, _baseReadDL);
            await ValidateUtil.CheckExist(entities, _baseReadDL);

            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                await _baseWriteDL.UpdateRangeAsync(entities);
                await AfterUpdate(entities);
                await _baseWriteDL.CommitTransactionAsync();
            }
            catch
            {
                await _baseWriteDL.RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Hàm hậu xử lý trước khi xóa
        /// </summary>
        protected virtual async Task AfterDelete(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hậu xử lý sau khi lưu thành công đối tượng
        /// </summary>
        protected virtual async Task AfterUpdate(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hậu xử lý sau khi lưu thành công đối tượng
        /// </summary>
        protected virtual async Task AfterInsert(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Tiền xử lý trước khi thêm mới (tự động gán PK và thông tin Audit)
        /// </summary>
        protected virtual async Task BeforeInsert(List<TEntity> entities)
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var pkPropName = metadata.PrimaryKeyName;
            var pkProp = metadata.Properties.FirstOrDefault(p => p.PropertyName == pkPropName);

            var now = DateTime.UtcNow;
            foreach (var entity in entities)
            {
                // Đảm bảo thêm mới ID nếu chưa có (chỉ áp dụng cho PK kiểu GUID)
                if (pkProp != null)
                {
                    Guid.TryParse(pkProp.PropertyInfo.GetValue(entity)?.ToString(), out Guid id);
                    if (id == Guid.Empty)
                    {
                        pkProp.PropertyInfo.SetValue(entity, Guid.NewGuid());
                    }
                }

                var user = _httpContextAccessor.HttpContext?.User;
                entity.CreatedBy = (user != null && user.Identity?.IsAuthenticated == true)
                    ? ClaimUtil.GetUserName(user)
                    : "System";
                entity.CreatedDate = now;
                entity.ModifiedDate = now;
                entity.IsDeleted = false;
            }
        }

        /// <summary>
        /// Tiền xử lý trước khi cập nhật (tự động gán thông tin Audit)
        /// </summary>
        protected virtual async Task BeforeUpdate(List<TEntity> entities)
        {
            var now = DateTime.UtcNow;
            var user = _httpContextAccessor.HttpContext?.User;
            foreach (var entity in entities)
            {
                entity.ModifiedBy = (user != null && user.Identity?.IsAuthenticated == true)
                    ? ClaimUtil.GetUserName(user)
                    : "System";
                entity.ModifiedDate = now;
                _baseWriteDL.SetChanged(entity, e => e.CreatedBy, false);
                _baseWriteDL.SetChanged(entity, e => e.CreatedDate, false);
            }
        }

        /// <summary>
        /// Lấy danh sách đối tượng theo biểu thức lambda chỉ định
        /// </summary>
        public async Task<List<TResult>> GetByCondition<TResult>(Expression<Func<TEntity, bool>> condition)
        {
            var parameters = new DynamicParameters();
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>();
            
            var extraGroup = ExpressionToFilterTranslator.Translate(condition);
            var request = new GetRequest { IsPaging = false };
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity, TResult>(request, parameters, extraGroup);

            var sql = $"{selectFromSql} {whereClause}";
            var items = await _baseReadDL.ExecuteQueryText<TResult>(sql, parameters);
            return [.. items];
        }

        /// <summary>
        /// Đếm thực thể theo điều kiện chỉ định (chỉ dùng cho nội bộ)
        /// </summary>
        public async Task<int> CountByCondition(Expression<Func<TEntity, bool>> condition)
        {
            var parameters = new DynamicParameters();
            var extraGroup = ExpressionToFilterTranslator.Translate(condition);
            var request = new GetRequest { IsPaging = false };
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity, TEntity>(request, parameters, extraGroup);
            
            var countSql = $"SELECT COUNT(*) {BuildSQLUtil.BuildFromClaude<TEntity, TEntity>()} {whereClause}";
            var totalCount = await _baseReadDL.ExecuteQueryToGetFirstResult<int>(countSql, parameters);
            return totalCount;
        }

        /// <summary>
        /// Lấy một đối tượng duy nhất theo điều kiện chỉ định sử dụng Dapper tối ưu hóa
        /// </summary>
        public async Task<TResult?> GetSingleByCondition<TResult>(Expression<Func<TEntity, bool>> condition)
        {
            var parameters = new DynamicParameters();
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>();
            
            var extraGroup = ExpressionToFilterTranslator.Translate(condition);
            var request = new GetRequest { IsPaging = false };
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity, TResult>(request, parameters, extraGroup);

            // Thêm giới hạn 1 dòng (LIMIT 1) để tối ưu hóa hiệu năng truy vấn của MySQL
            var sql = $"{selectFromSql} {whereClause} LIMIT 1";
            return await _baseReadDL.ExecuteQuerySingle<TResult>(sql, parameters);
        }

        
    }
}

