using Dapper;
using HoangCN.BL.Interfaces;
using HoangCN.BL.Utils;
using HoangCN.Common.Attributes;
using HoangCN.Common.Base;
using HoangCN.Common.Enums;
using HoangCN.Common.Exceptions;
using HoangCN.Common.Model.DTOs;
using HoangCN.Common.Model.Requests;
using HoangCN.Common.Utils;
using HoangCN.DL.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace HoangCN.BL.Base
{
    /// <summary>
    /// Lớp triển khai cho giao diện cơ sở tầng nghiệp vụ
    /// </summary>
    public class BaseBL<TEntity> : IBaseBL<TEntity> where TEntity : BaseEntity
    {
        protected readonly IBaseDL _baseDL;

        public BaseBL(IBaseDL baseDL)
        {
            _baseDL = baseDL;
        }

        /// <summary>
        /// Xóa nhiều đối tượng thông qua danh sách ID
        /// </summary>
        public async Task Delete(DeleteRequest request)
        {
            // Lấy danh sách entity ra
            var res = await Get<TEntity>(new GetRequest { Ids = request.Ids });

            if (res.Items.Count == 0)
            {
                return;
            }

            // Xử lý trước khi xóa
            BeforeDelete(res.Items);

            // Xóa
            await Save(res.Items);
        }


        /// <summary>
        /// Lấy danh sách đối tượng
        /// </summary>
        public async Task<ResultDto<TResult>> Get<TResult>(GetRequest request)
        {
            var parameters = new DynamicParameters();
            var mainTableName = typeof(TEntity).Name;
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>();
            
            // Xây dựng các điều kiện lọc WHERE
            var whereClause = BuildSQLUtil.BuildWhereClaude<TEntity>(request, parameters);

            // Xây dựng mệnh đề ORDER BY
            var sortClause = BuildSQLUtil.BuildSortClaude<TEntity>(request);

            var result = new ResultDto<TResult>();

            if (request.IsPaging)
            {
                // 1. Tính tổng số dòng (COUNT)
                var countSql = $"SELECT COUNT(*) FROM `{mainTableName}` {whereClause}";
                var totalCount = await _baseDL.ExecuteQueryToGetFirstResult<int>(countSql, parameters);
                result.Total = totalCount;
                result.Page = request.Page ?? 1;
                result.Size = request.Size ?? 10;

                // 2. Lấy danh sách phần tử (PAGING)
                var pagingSql = $"{selectFromSql} {whereClause} {sortClause}" + BuildSQLUtil.BuildPagingClaude(result.Page, result.Size, parameters);

                var items = await _baseDL.ExecuteQueryText<TResult>(pagingSql, parameters);
                result.Items = items.ToList();
            }
            else
            {
                var sql = $"{selectFromSql} {whereClause} {sortClause}";
                var items = await _baseDL.ExecuteQueryText<TResult>(sql, parameters);
                result.Items = items.ToList();
                result.Total = result.Items.Count;
                result.Page = 1;
                result.Size = result.Total > 0 ? result.Total : 1;
            }

            return result;
        }


        /// <summary>
        /// Thêm/Sửa/Xóa các đối tượng theo State
        /// </summary>
        public async Task Save(List<TEntity> entities)
        {
            if (entities is null || entities.Count == 0)
            {
                throw new BadRequestException("Dữ liệu trống hoặc sai định dạng");
            }

            await BeforeSave(entities);

            await _baseDL.StartTransaction();
            try
            {
                // Xử lý thêm mới / cập nhật
                var insertOfUpdateEntities = entities.Where(e => e.State == ModelState.Insert || e.State == ModelState.Update).ToList();
                if (insertOfUpdateEntities?.Count > 0)
                {
                    var sql = BuildSQLUtil.BuildQueryStringInsertOrUpdate(insertOfUpdateEntities, out var param);
                    await _baseDL.ExecuteCommandText(sql, param);
                }

                // Xử lý xóa
                var deleteEntities = entities.Where(e => e.State == ModelState.Delete).ToList();
                if (deleteEntities?.Count > 0)
                {
                    var sql = BuildSQLUtil.BuildQueryStringDelete(deleteEntities, out var param);
                    await _baseDL.ExecuteCommandText(sql, param);
                }

                // Xử lý thêm sau khi thay đổi các đối tượng
                await AfterSave(entities);

                // Xác nhận giao dịch
                await _baseDL.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Back lại các thay đổi
                await _baseDL.RollbackTransaction();
                throw;
            }
        }


        /// <summary>
        /// Hàm tiền xử lý trước khi xóa
        /// </summary>
        protected virtual void BeforeDelete(List<TEntity> entities)
        {
            // Cập nhật trạng thái của tất cả entity là cần xóa
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
        /// Tiền xử lý trước khi lưu
        /// </summary>
        protected virtual async Task BeforeSave(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                // Cập nhật trạng thái entities
                if (entity is null)
                {
                    continue;
                }

                if (entity.State == ModelState.Delete)
                {
                    continue;
                }

                // Đã gắn cờ trạng thái rồi thì bỏ qua
                if (entity.State != ModelState.None)
                {
                    continue;
                }

                // ID null => Insert, không null => Update
                var pkPropName = $"{entity.GetType().Name}Id";
                Guid.TryParse(entity.GetValueByPropName(pkPropName) + "", out Guid id);
                if (id == Guid.Empty)
                {
                    entity.SetValueByPropName(pkPropName, Guid.NewGuid());
                    entity.State = ModelState.Insert;
                }
                else
                {
                    entity.State = ModelState.Update;
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

                // Validate từng entity
                await Validate(entity);
            }
        }


        /// <summary>
        /// Kiểm tra các thuộc tính
        /// </summary>
        protected virtual async Task Validate(TEntity entity)
        {
            var props = entity.GetType().GetProperties();
            foreach (var prop in props)
            {
                var v = prop.GetValue(entity);
                var pkPropName = $"{entity.GetType().Name}Id";
                Guid.TryParse(entity.GetValueByPropName(pkPropName)?.ToString(), out Guid id);

                if (id == Guid.Empty)
                {
                    throw new ServerErrorException("ID không hợp lệ");
                }

                // Lấy tên hiển thị thân thiện
                var displayName = prop.GetCustomAttribute<DisplayNameAttribute>(true)?.DisplayName 
                                  ?? prop.GetCustomAttribute<DisplayAttribute>(true)?.GetName() 
                                  ?? prop.Name;

                // Kiểm tra khóa chính phải tồn tại khi đang cập nhật
                var isKeyUpdateCheck = entity.State == ModelState.Update && prop.GetCustomAttribute<KeyAttribute>(true) != null;
                if (isKeyUpdateCheck)
                {
                    var query = BuildSQLUtil.BuildQueryStringCountDuplicate<TEntity>(null, pkPropName, id, out var param);
                    var dupCount = await _baseDL.ExecuteQueryToGetFirstResult<int>(query, param);
                    if (dupCount == 0)
                    {
                        throw new ServerErrorException("Đối tượng được cập nhật không tồn tại");
                    }
                }

                // Kiểm tra null hoặc trống bằng [Required] mặc định
                var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>(true);
                var isNullOrEmpty = v is null ||
                                    v is string strV && string.IsNullOrEmpty(strV) ||
                                    v is Guid guidV && guidV == Guid.Empty;

                if (isNullOrEmpty)
                {
                    if (requiredAttr != null)
                    {
                        var msg = !string.IsNullOrEmpty(requiredAttr.ErrorMessage) 
                            ? requiredAttr.FormatErrorMessage(displayName) 
                            : $"Trường {displayName} không được phép để trống.";
                        throw new BadRequestException(msg);
                    }

                    // Nếu trường này null/trống và không bắt buộc (không có [Required]), bỏ qua tất cả các check sau!
                    continue;
                }

                // Kiểm tra độ dài bằng [StringLength] mặc định
                var stringLengthAttr = prop.GetCustomAttribute<StringLengthAttribute>(true);
                if (stringLengthAttr != null && v is string sValue)
                {
                    if (sValue.Length < stringLengthAttr.MinimumLength ||
                        sValue.Length > stringLengthAttr.MaximumLength)
                    {
                        var msg = !string.IsNullOrEmpty(stringLengthAttr.ErrorMessage)
                            ? stringLengthAttr.FormatErrorMessage(displayName)
                            : $"Độ dài của trường {displayName} không hợp lệ.";
                        throw new BadRequestException(msg);
                    }
                }

                // Kiểm tra trùng lặp (cho cả Id nếu đang cập nhật)
                var checkDuplicateAttr = prop.GetCustomAttribute<CheckExistAttribute>(true);
                if (checkDuplicateAttr != null)
                {
                    Guid? ignoreId = null;

                    // Nếu đang cập nhật thì phải bỏ qua check trùng với dòng hiện tại
                    if (entity.State == ModelState.Update)
                    {
                        ignoreId = id;
                    }

                    // Xác định thực thể đích cần kiểm tra
                    var queryEntityType = checkDuplicateAttr.TargetEntity ?? typeof(TEntity);
                    var queryColumnName = prop.Name;

                    // Nếu là khóa ngoại, ta kiểm tra giá trị khóa ngoại đó có tồn tại ở khóa chính của bảng đích không
                    if (checkDuplicateAttr.TargetEntity != null)
                    {
                        queryColumnName = $"{checkDuplicateAttr.TargetEntity.Name}Id";
                        ignoreId = null; // Khóa ngoại thì không bỏ qua hàng hiện tại
                    }

                    // Truy vấn đếm
                    var query = BuildSQLUtil.BuildQueryStringCountDuplicate(queryEntityType, ignoreId, queryColumnName, prop.GetValue(entity), out var param);
                    var dupCount = await _baseDL.ExecuteQueryToGetFirstResult<int>(query, param);
                    if (checkDuplicateAttr.MustExist && dupCount == 0 ||
                        !checkDuplicateAttr.MustExist && dupCount > 0)
                    {
                        var msg = checkDuplicateAttr.ErrorMessage;
                        if (!string.IsNullOrEmpty(msg) && msg.Contains("{0}"))
                        {
                            msg = string.Format(msg, displayName);
                        }
                        throw new BadRequestException(msg);
                    }
                }
            }
        }

        /// <summary>
        /// Lấy đối tượng theo ID
        /// </summary>
        public async Task<TResult> GetById<TResult>(Guid id)
        {
            var request = GetRequest.GetByIdRequest(id);
            var res = await Get<TResult>(request);
            return res.Items.FirstOrDefault();
        }

        /// <summary>
        /// Lấy danh sách đối tượng thỏa mãn điều kiện chỉ định
        /// </summary>
        public async Task<List<TResult>> GetByCondition<TResult>(Expression<Func<TEntity, bool>> condition)
        {
            var selectFromSql = BuildSQLUtil.BuildSelectClaude<TEntity, TResult>();
            var whereClause = BuildSQLUtil.BuildWhereClauseFromExpression<TEntity>(condition, out var parameters);
            
            var sql = $"{selectFromSql} {whereClause}";
            var items = await _baseDL.ExecuteQueryText<TResult>(sql, parameters);
            return items.ToList();
        }
    }
}
