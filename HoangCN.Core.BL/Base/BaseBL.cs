using Dapper;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Utils;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Metadata;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.Core.DL.Utils;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace HoangCN.Core.BL.Base
{
    /// <summary>
    /// Lớp triển khai cho giao diện cơ sở tầng nghiệp vụ sử dụng kết hợp EF Core và Dapper
    /// </summary>
    public class BaseBL<TEntity> : IBaseBL<TEntity> where TEntity : BaseEntity, new()
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
        /// Thêm mới danh sách entity tự động đệ quy
        /// </summary>
        public async Task InsertEntities(List<TEntity> handingEntities)
        {
            await HandleBeforeInsert(handingEntities);
            ValidateUtil.CommonValidate(handingEntities);
            var changedEntities = await _baseWriteDL.SaveEntities(handingEntities);
            await HandleAfterInsert(changedEntities);
        }

        /// <summary>
        /// Cập nhật danh sách entity tự động đệ quy và đồng bộ khóa ngoại
        /// </summary>
        public async Task UpdateEntities(List<TEntity> entities)
        {
            await HandleBeforeUpdate(entities);
            ValidateUtil.CommonValidate(entities);
            var changedEntities = await _baseWriteDL.SaveEntities(entities);
            await HandleAfterUpdate(changedEntities);
        }

        /// <summary>
        /// Xóa danh sách thực thể
        /// </summary>
        public async Task DeleteEntities(DeleteRequest request)
        {
            List<TEntity> entityWrappers = [];
            entityWrappers.AddRange(request.Ids.Select(id =>
            {
                var entity = new TEntity
                {
                    State = ModalState.Delete
                };
                entity.SetValueByPropName($"{typeof(TEntity).Name}Id", id);

                return entity;
            }));
            await HandleBeforeDelete(entityWrappers);
            var changedEntities = await _baseWriteDL.SaveEntities(entityWrappers);
            await HandleAfterDelete(changedEntities);
        }

        /// <summary>
        /// Xử lý sau khi xóa
        /// </summary>
        protected virtual async Task HandleAfterDelete(List<TEntity> changedEntities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý trước khi xóa
        /// </summary>
        protected virtual async Task HandleBeforeDelete(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý khi xóa bị lỗi
        /// </summary>
        protected virtual async Task HandleOnDeleteFail(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý sau khi thêm mới
        /// </summary>
        protected virtual async Task HandleAfterInsert(List<TEntity> changedEntities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý trước khi thêm mới
        /// </summary>
        protected virtual async Task HandleBeforeInsert(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý khi thêm mới thất bại
        /// </summary>
        protected virtual async Task HandleInsertFail(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý sau khi cập nhật
        /// </summary>
        protected virtual async Task HandleAfterUpdate(List<TEntity> changedEntities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý trước khi cập nhật
        /// </summary>
        protected virtual async Task HandleBeforeUpdate(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý khi cập nhật thất bại
        /// </summary>
        protected virtual async Task HandleUpdateFail(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Lấy danh sách đối tượng sử dụng Dapper (Read DB)
        /// </summary>
        public async Task<ResultDto<TResult>> Get<TResult>(
            GetRequest request, 
            Expression<Func<TEntity, bool>>? condition = null,
            Expression<Func<TEntity, object>>? selector = null)
        {
            var parameters = new DynamicParameters();
            var mainTableName = typeof(TEntity).Name;
            var selectedCols = selector != null ? ExpressionToSelectColumnsTranslator.Translate(selector) : null;
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>(selectedCols);
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity, TResult>(
                request, 
                parameters,
                condition != null? ExpressionToFilterTranslator.Translate(condition) : null);
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

            return await GetFirstByCondition<TResult>(lambda);
        }


        /// <summary>
        /// Lấy danh sách đối tượng theo biểu thức lambda chỉ định
        /// </summary>
        public async Task<List<TResult>> GetByCondition<TResult>(
            Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, object>>? selector = null)
        {
            var parameters = new DynamicParameters();
            var selectedCols = selector != null ? ExpressionToSelectColumnsTranslator.Translate(selector) : null;
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>(selectedCols);
            
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
        public async Task<TResult?> GetFirstByCondition<TResult>(
            Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, object>>? selector = null)
        {
            var parameters = new DynamicParameters();
            var selectedCols = selector != null ? ExpressionToSelectColumnsTranslator.Translate(selector) : null;
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>(selectedCols);
            
            var extraGroup = ExpressionToFilterTranslator.Translate(condition);
            var request = new GetRequest { IsPaging = false };
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity, TResult>(request, parameters, extraGroup);

            // Thêm giới hạn 1 dòng (LIMIT 1) để tối ưu hóa hiệu năng truy vấn của MySQL
            var sql = $"{selectFromSql} {whereClause} LIMIT 1";
            return await _baseReadDL.ExecuteQuerySingle<TResult>(sql, parameters);
        }
    }
}

