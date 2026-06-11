using Dapper;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Metadata;
using HoangCN.Core.BL.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace HoangCN.Core.BL.Base
{
    /// <summary>
    /// Lớp triển khai cho giao diện cơ sở tầng nghiệp vụ sử dụng kết hợp EF Core và Dapper
    /// </summary>
    public class BaseBL<TEntity> : IBaseBL<TEntity> where TEntity : BaseEntity
    {
        protected readonly IBaseReadDL _baseReadDL;
        protected readonly IBaseWriteDL _baseWriteDL;

        public BaseBL(IBaseReadDL baseReadDL, IBaseWriteDL baseWriteDL)
        {
            _baseReadDL = baseReadDL ?? throw new ArgumentNullException(nameof(baseReadDL));
            _baseWriteDL = baseWriteDL ?? throw new ArgumentNullException(nameof(baseWriteDL));
        }

        /// <summary>
        /// Xóa nhiều đối tượng thông qua danh sách ID
        /// </summary>
        public async Task Delete(DeleteRequest request)
        {
            var res = await Get<TEntity>(new GetRequest { Ids = request.Ids });
            if (res.Items.Count == 0)
            {
                return;
            }

            BeforeDelete(res.Items);
            await Save(res.Items);
        }

        /// <summary>
        /// Lấy danh sách đối tượng sử dụng Dapper (Read DB)
        /// </summary>
        public async Task<ResultDto<TResult>> Get<TResult>(GetRequest request)
        {
            var parameters = new DynamicParameters();
            var mainTableName = typeof(TEntity).Name;
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>();
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity>(request, parameters);
            var sortClause = BuildSQLUtil.BuildSortClaude<TEntity>(request);

            var result = new ResultDto<TResult>();

            if (request.IsPaging)
            {
                // 1. Tính tổng số dòng (COUNT) sử dụng Dapper
                var countSql = $"SELECT COUNT(*) FROM `{mainTableName}` {whereClause}";
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
        /// Thêm/Sửa/Xóa các đối tượng theo State sử dụng EF Core (Write DB)
        /// </summary>
        public async Task Save(List<TEntity> entities)
        {
            if (entities is null || entities.Count == 0)
            {
                throw new BadRequestException("Dữ liệu trống hoặc sai định dạng");
            }

            await BeforeSave(entities);

            // Bắt đầu EF Core Transaction trên Write DB
            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                // Lưu thông tin bằng EF Core
                await _baseWriteDL.SaveEntitiesAsync(entities);
                
                await AfterSave(entities);

                // Xác nhận giao dịch
                await _baseWriteDL.CommitTransactionAsync();
            }
            catch
            {
                // Hoàn tác giao dịch lỗi
                await _baseWriteDL.RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Hàm tiền xử lý trước khi xóa
        /// </summary>
        protected virtual void BeforeDelete(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.State = ModelState.Delete;
            }
        }

        /// <summary>
        /// Hậu xử lý sau khi lưu thành công đối tượng
        /// </summary>
        protected virtual async Task AfterSave(List<TEntity> entities)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Tiền xử lý trước khi lưu thực thể (tự động điền PK và các trường Audit)
        /// </summary>
        protected virtual async Task BeforeSave(List<TEntity> entities)
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var pkPropName = metadata.PrimaryKeyName;

            foreach (var entity in entities)
            {
                if (entity is null || entity.State == ModelState.Delete) continue;
                if (entity.State != ModelState.None) continue;

                var pkProp = metadata.Properties.FirstOrDefault(p => p.PropertyName == pkPropName);
                if (pkProp != null)
                {
                    Guid.TryParse(pkProp.PropertyInfo.GetValue(entity)?.ToString(), out Guid id);
                    if (id == Guid.Empty)
                    {
                        pkProp.PropertyInfo.SetValue(entity, Guid.NewGuid());
                        entity.State = ModelState.Insert;
                    }
                    else
                    {
                        entity.State = ModelState.Update;
                    }
                }

                if (entity.State == ModelState.Insert)
                {
                    entity.CreatedBy = "Hoàng Cao Nguyên";
                    entity.CreatedDate = DateTime.Now;
                    entity.ModifiedDate = entity.CreatedDate;
                }
                else
                {
                    entity.ModifiedBy = "Hoàng Cao Nguyên";
                    entity.ModifiedDate = DateTime.Now;
                }
            }

            // Thực thi kiểm tra hợp lệ gom cụm tối ưu
            await ValidateBulk(entities);
        }

        /// <summary>
        /// Kiểm tra hợp lệ gom nhóm cho toàn bộ danh sách, loại bỏ N+1 query và C# reflection overhead
        /// </summary>
        protected virtual async Task ValidateBulk(List<TEntity> entities)
        {
            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));
            var pkPropName = metadata.PrimaryKeyName;

            // 1. Kiểm tra các điều kiện trong bộ nhớ (Required, StringLength) trước - Hoàn toàn không gọi DB
            foreach (var entity in entities)
            {
                if (entity.State == ModelState.Delete) continue;

                foreach (var prop in metadata.Properties)
                {
                    // Không kiểm tra các trường audit của người tạo lúc cập nhật vì chúng sẽ được giữ nguyên ở DB
                    if (entity.State == ModelState.Update && (prop.PropertyName == "CreatedBy" || prop.PropertyName == "CreatedDate"))
                    {
                        continue;
                    }

                    var v = prop.PropertyInfo.GetValue(entity);

                    // Kiểm tra null hoặc trống [Required]
                    if (prop.RequiredAttr != null)
                    {
                        var isNullOrEmpty = v is null ||
                                            v is string strV && string.IsNullOrEmpty(strV) ||
                                            v is Guid guidV && guidV == Guid.Empty;

                        if (isNullOrEmpty)
                        {
                            var msg = !string.IsNullOrEmpty(prop.RequiredAttr.ErrorMessage)
                                ? prop.RequiredAttr.FormatErrorMessage(prop.DisplayName)
                                : $"Trường {prop.DisplayName} không được phép để trống.";
                            throw new BadRequestException(msg);
                        }
                    }

                    // Kiểm tra độ dài [StringLength]
                    if (prop.StringLengthAttr != null && v is string sValue)
                    {
                        if (sValue.Length < prop.StringLengthAttr.MinimumLength ||
                            sValue.Length > prop.StringLengthAttr.MaximumLength)
                        {
                            var msg = !string.IsNullOrEmpty(prop.StringLengthAttr.ErrorMessage)
                                ? prop.StringLengthAttr.FormatErrorMessage(prop.DisplayName)
                                : $"Độ dài của trường {prop.DisplayName} không hợp lệ.";
                            throw new BadRequestException(msg);
                        }
                    }
                }
            }

            // 2. Kiểm tra khóa chính phải tồn tại khi UPDATE (Gom cụm thành 1 truy vấn duy nhất)
            var updateEntities = entities.Where(e => e.State == ModelState.Update).ToList();
            if (updateEntities.Count > 0)
            {
                var pkProp = metadata.Properties.FirstOrDefault(p => p.PropertyName == pkPropName);
                if (pkProp != null)
                {
                    var idsToCheck = updateEntities.Select(e => {
                        Guid.TryParse(pkProp.PropertyInfo.GetValue(e)?.ToString(), out Guid id);
                        return id;
                    }).Where(id => id != Guid.Empty).Distinct().ToList();

                    if (idsToCheck.Count > 0)
                    {
                        var sql = BuildSQLUtil.BuildQueryStringCheckPkExists(metadata.EntityType.Name, pkPropName);
                        var param = new DynamicParameters();
                        param.Add("Ids", idsToCheck);

                        var existingIds = (await _baseReadDL.ExecuteQueryText<Guid>(sql, param)).ToHashSet();

                        foreach (var entity in updateEntities)
                        {
                            Guid.TryParse(pkProp.PropertyInfo.GetValue(entity)?.ToString(), out Guid id);
                            if (!existingIds.Contains(id))
                            {
                                throw new BadRequestException($"Đối tượng {metadata.EntityType.Name} có mã {id} được sửa đổi không tồn tại.");
                            }
                        }
                    }
                }
            }

            // 3. Kiểm tra trùng lặp và sự tồn tại khóa ngoại (CheckExistAttribute) gom nhóm
            foreach (var prop in metadata.Properties)
            {
                var checkExistAttr = prop.CheckExistAttr;
                if (checkExistAttr == null) continue;

                var targetEntityType = checkExistAttr.TargetEntity ?? metadata.EntityType;
                var targetTableName = targetEntityType.Name;
                var queryColumnName = prop.PropertyName;

                if (checkExistAttr.TargetEntity != null)
                {
                    queryColumnName = $"{checkExistAttr.TargetEntity.Name}Id";
                }

                var activeEntities = entities.Where(e => e.State != ModelState.Delete).ToList();
                var valuesToCheck = activeEntities
                    .Select(e => prop.PropertyInfo.GetValue(e))
                    .Where(v => v != null && !(v is Guid g && g == Guid.Empty) && !(v is string s && string.IsNullOrEmpty(s)))
                    .Distinct()
                    .ToList();

                if (valuesToCheck.Count == 0) continue;

                var param = new DynamicParameters();
                param.Add("Values", valuesToCheck);

                if (checkExistAttr.TargetEntity == null)
                {
                    // Kiểm tra trùng lặp cột trên chính bảng này (loại trừ trường hợp so trùng với chính hàng đang sửa)
                    var sql = BuildSQLUtil.BuildQueryStringCheckDuplicate(targetTableName, pkPropName, queryColumnName);
                    var existingRecords = await _baseReadDL.ExecuteQueryText<dynamic>(sql, param);

                    foreach (var entity in activeEntities)
                    {
                        var val = prop.PropertyInfo.GetValue(entity);
                        if (val == null) continue;

                        Guid.TryParse(metadata.Properties.FirstOrDefault(p => p.PropertyName == pkPropName)?.PropertyInfo.GetValue(entity)?.ToString(), out Guid currentId);

                        var isDuplicate = existingRecords.Any(r => 
                        {
                            var dict = (IDictionary<string, object>)r;
                            var recordVal = dict[queryColumnName]?.ToString();
                            var recordId = Guid.Parse(dict[pkPropName].ToString()!);
                            return recordVal == val.ToString() && recordId != currentId;
                        });

                        if (isDuplicate)
                        {
                            var msg = string.Format(checkExistAttr.ErrorMessage ?? "Trường {0} đã tồn tại trong hệ thống.", prop.DisplayName);
                            throw new BadRequestException(msg);
                        }
                    }
                }
                else
                {
                    // Kiểm tra khóa ngoại trên bảng khác có tồn tại khóa chính tương ứng
                    var sql = BuildSQLUtil.BuildQueryStringCheckFkExists(targetTableName, queryColumnName);
                    var existingRecords = await _baseReadDL.ExecuteQueryText<dynamic>(sql, param);
                    var existingValues = existingRecords
                        .Select(r => 
                        {
                            var dict = (IDictionary<string, object>)r;
                            return dict.Values.FirstOrDefault()?.ToString();
                        })
                        .Where(v => v != null)
                        .ToHashSet()!;

                    foreach (var entity in activeEntities)
                    {
                        var val = prop.PropertyInfo.GetValue(entity);
                        if (val == null) continue;

                        var exists = existingValues.Contains(val.ToString()!);
                        if (checkExistAttr.MustExist && !exists)
                        {
                            var msg = string.Format(checkExistAttr.ErrorMessage ?? "Trường {0} không tồn tại trong hệ thống.", prop.DisplayName);
                            throw new BadRequestException(msg);
                        }
                        else if (!checkExistAttr.MustExist && exists)
                        {
                            var msg = string.Format(checkExistAttr.ErrorMessage ?? "Trường {0} đã tồn tại trong hệ thống.", prop.DisplayName);
                            throw new BadRequestException(msg);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Lấy một đối tượng theo ID
        /// </summary>
        public async Task<TResult?> GetById<TResult>(Guid id)
        {
            var request = GetRequest.GetByIdRequest(id);
            var res = await Get<TResult>(request);
            return res.Items.FirstOrDefault();
        }

        /// <summary>
        /// Lấy danh sách đối tượng theo biểu thức lambda chỉ định
        /// </summary>
        public async Task<List<TResult>> GetByCondition<TResult>(Expression<Func<TEntity, bool>> condition)
        {
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>();
            var whereClause = BuildSQLUtil.BuildWhereClauseFromExpression<TEntity>(condition, out var parameters);
            
            var sql = $"{selectFromSql} {whereClause}";
            var items = await _baseReadDL.ExecuteQueryText<TResult>(sql, parameters);
            return items.ToList();
        }
    }
}

