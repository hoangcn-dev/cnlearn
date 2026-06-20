#nullable enable
using Dapper;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Metadata;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.Core.DL.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using static Dapper.SqlMapper;

namespace HoangCN.Core.DL.Implementation
{
    /// <summary>
    /// Triển khai cơ chế ghi dữ liệu đảm bảo tính toàn vẹn (ACID) sử dụng EF Core
    /// </summary>
    public class BaseWriteDL : IBaseWriteDL
    {
        private readonly DbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDbContextTransaction? _transaction;
        private int _transactionCount = 0;

        public BaseWriteDL(DbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TEntity>> SaveEntities<TEntity, TParentEntity>(List<TEntity> entities, TParentEntity? parent = null) 
            where TParentEntity : BaseEntity
            where TEntity : BaseEntity
        {
            if (entities.Count == 0)
            {
                return [];
            }

            /// Bước 1: Tự động gắn cờ xóa/cập nhật cho entities (nếu chưa được chỉ định)
            await SetModalState(entities);

            /// Bước 2: Gắn giá trị cho các trường Audit
            AssignAuditProperties(entities);
            var insertEntities = entities.Where(e => e.State == ModalState.Insert).ToList();
            var deleteEntities = entities.Where(e => e.State == ModalState.Delete).ToList();
            var updateEntities = entities.Where(e => e.State == ModalState.Update).ToList();

            /// Bước 3: Kiểm tra trùng lặp, kiểm tra tồn tại
            await CheckExist(entities);

            /// Bước 4: Lấy lên danh sách thực thể đã được lưu dưới DB
            // Dùng để so sánh thay đổi các trường (Update) và xác định các entities cần xóa
            var savedEntites = await GetSavedEntities(entities, parent);

            /// Bước 5: Xác định những trường bị thay đổi giá trị đối với các entity dạng Update
            // Dùng để tối ưu câu SQL cập nhật được sinh ra.
            DetermineChangedProperty(updateEntities, savedEntites);

            /// Bước 6: Bổ xung thêm những phần tử cần xóa 
            // Khi cập nhật Childrens, những phần tử trong DB khi không được đề cập đến => Tự động xóa
            if (parent != null)
            {
                var remainingIds = updateEntities.Select(e => e.GetId());
                foreach (var savedEntity in savedEntites)
                {
                    if (!remainingIds.Contains(savedEntity.GetId()))
                    {
                        deleteEntities.Add(savedEntity);
                    }
                }
            }

            /// Bước 7: Lưu thay đổi bằng transaction
            await BeginTransactionAsync();
            try
            {
                // Thêm những phần tử mới
                await _context.Set<TEntity>().AddRangeAsync(insertEntities);

                // Xóa những đứa cần xóa
                var deleteSql = $"DELETE FROM {typeof(TEntity).Name} " +
                    $"WHERE {typeof(TEntity).Name}Id IN ({string.Join(",", deleteEntities.Select((_, index) => $"{{{index}}}"))})";
                await _context.Database.ExecuteSqlRawAsync(deleteSql, deleteEntities.Select(e => e.GetId()));

                // Lưu
                await _context.SaveChangesAsync();

                await CommitTransactionAsync();

                // Trả về danh sách
                return insertEntities
                    .Concat(updateEntities)
                    .Concat(deleteEntities)
                    .ToList();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        private void DetermineChangedProperty<TEntity>(List<TEntity> updateEntities, List<TEntity> savedEntites) where TEntity : BaseEntity
        {
            foreach (var entity in updateEntities)
            {
                var idPropName = ReflectionUtil.GetIdPropName<TEntity>();
                var saveEntity = savedEntites.FirstOrDefault(e =>
                    _context.Entry(e).Property(idPropName).CurrentValue.Equals(entity.GetId()));

                if (saveEntity != null)
                {
                    _context.Entry(saveEntity).CurrentValues.SetValues(entity);
                }
            }
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của các khóa chính (Primary Key) trong cơ sở dữ liệu khi cập nhật.
        /// </summary>
        private async Task ValidatePkExists<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity
        {
            if (entities == null || !entities.Any()) return;

            var tableName = $"{typeof(TEntity).Name}";
            var checkSql = @$"
                SELECT {tableName}Id 
                FROM {tableName} 
                WHERE {tableName}Id IN ({string.Join(",", entities.Select((_, index) => $"{{{index}}}"))})";
            
            var checkIds = entities.Select(e => e.GetId());
            var existingIds = await _context.Database
                .SqlQueryRaw<Guid>(checkSql, checkIds)
                .ToListAsync();

            // Kết luận
            foreach (var id in checkIds)
            {
                if (existingIds.Contains(id)) continue;
                throw new NotFoundException($"Thực thể {id} không tồn tại");
            }
        }


        /// <summary>
        /// CheckExist: Kiểm tra sự tồn tại hoặc không trùng lặp của các bản ghi trong cơ sở dữ liệu
        /// dựa trên các thuộc tính [CheckExist]/[Key]/[FK] đã được định nghĩa trong lớp thực thể.
        /// </summary>
        private async Task CheckExist<TEntity>(List<TEntity> entities)
            where TEntity : BaseEntity
        {
            if (!entities.Any()) return;

            var metadata = EntityMetadataCache.GetMetadata(typeof(TEntity));

            foreach (var prop in metadata.Properties)
            {
                /// Bước 1: Lọc chỉ xử lý các attr liên quan đến Check Exist/Khóa ngoại/Khóa chính
                var checkExistAttr = prop.CheckExistAttr;
                var fkAttr = prop.FKAttr;
                var isFkProp = prop.PropertyName == ReflectionUtil.GetIdPropName<TEntity>();
                if (checkExistAttr == null && fkAttr == null && !isFkProp) continue;

                /// Bước 2: Thu thập các giá trị cần kiểm tra từ dữ liệu đầu vào (cần lưu lại thứ tự để xác định vị trí entity bị lỗi sau này)
                var checkValues = new Dictionary<object, List<int>>();
                for ( var i = 0; i < entities.Count; i++)
                {
                    if (entities[0].State == ModalState.None) continue;

                    var value = entities[i].GetValueByPropName(prop.PropertyName);
                    if (value == null) continue;
                    if (!checkValues.ContainsKey(value)) checkValues.Add(value, new List<int>());

                    // Trường hợp check khóa ngoại tồn tại, chỉ kiểm tra khi đang thêm mới/cập nhật
                    if (fkAttr != null && (entities[i].State == ModalState.Insert || entities[i].State == ModalState.Update))
                    {
                        checkValues[value].Add(i + 1);
                    }

                    // Trường hợp check key tồn tại, chỉ kiểm tra khi đang xóa/cập nhật
                    if (isFkProp && (entities[i].State == ModalState.Delete || entities[i].State == ModalState.Update))
                    {
                        checkValues[value].Add(i + 1);
                    }

                    // Trường hợp check các trường khác, chỉ kiểm tra khi không phải là thao tác xóa
                    if (checkExistAttr != null && entities[i].State != ModalState.Delete)
                    {
                        checkValues[value].Add(i + 1);
                    }
                }

                // Chỉ xét các giá trị cần thiết
                checkValues = checkValues.Where(kvp => kvp.Value.Count > 0).ToDictionary();
                if (checkValues.Count == 0) continue;

                /// Bước 4: Liệt kê các thông tin chung cần thiết cho quá rình kiêm tra
                // Bảng sẽ kiểm tra
                var targetTableName = 
                    checkExistAttr?.TargetEntity?.Name ?? 
                    fkAttr?.TargetEntity?.Name ??
                    metadata.EntityType.Name;

                // Tên thuộc tính sẽ kiểm tra
                var checkColumnName = prop.PropertyName;

                /// Bước 5: Kiểm tra đảm bảo giá trị tồn tại 
                if ((checkExistAttr != null && checkExistAttr.MustExist) || fkAttr != null || isFkProp)
                {
                    // Câu SQL truy vấn lấy danh sách giá trị ra
                    var sql = @$"
                        SELECT {checkColumnName} FROM {targetTableName}
                        WHERE {checkColumnName} IN ({string.Join(", ", checkValues.Keys.Select((_, index) => $"{{{index}}}"))})";
                    var existingValues = _context.Database.SqlQueryRaw<object>(sql, checkValues.Keys);

                    if (existingValues.Count() == checkValues.Count) continue;

                    // Tạo thông báo lỗi
                    var missValues = checkValues.Keys.Except(existingValues);
                    var missIndexs = missValues.SelectMany(v => checkValues[v]);
                    var missMessage = entities.Count == 1?
                        $"Trường {prop.PropertyInfo.GetPropDisplayName()} của {ReflectionUtil.GetEntityDisplayName<TEntity>()} không tồn tại":
                        $"Trường {prop.PropertyInfo.GetPropDisplayName()} của {ReflectionUtil.GetEntityDisplayName<TEntity>()} ({string.Join(", ", missIndexs)}) không tồn tại"; 

                    throw new BadRequestException(missMessage);
                }

                /// Bước 6: Kiểm tra đảm bảo giá trị không tồn tại (không bị trùng)
                else if ((checkExistAttr != null && !checkExistAttr.MustExist))
                {
                    // Kiểm tra trùng ngay trên list hiện tại
                    if (checkValues.Values.Any(indexs => indexs.Count > 1))
                    {
                        throw new BadRequestException(
                            $"Trường {prop.PropertyInfo.GetPropDisplayName()} của {ReflectionUtil.GetEntityDisplayName<TEntity>()} bị trùng lặp trên danh sách");
                    }

                    // Kiểm tra trùng ngay dưới database
                    // Tạo câu lệnh truy vấn cần lấy thêm cả Id của bản ghi trùng để kiểm tra trường hợp trùng với chính nó khi UPDATE
                    var sql = @$"
                        SELECT {targetTableName}Id, {checkColumnName} FROM {targetTableName}
                        WHERE {checkColumnName} IN ({string.Join(", ", checkValues.Select((_, index) => $"{{{index}}}"))})";
                    var result = await _context.Database.GetDbConnection().QueryAsync(sql, checkValues.Keys);
                    var map = result.ToDictionary(
                        row => row.GetValueByPropName(checkColumnName),
                        row => Guid.Parse(row.GetValueByPropName($"{targetTableName}Id").ToString()));

                    var duplicateIndexs = new List<int>();
                    foreach (var row in result)
                    {
                        var value = row.GetValueByPropName(checkColumnName) as object;
                        var id = Guid.Parse(row.GetValueByPropName($"{targetTableName}Id").ToString());
                        var checkEntityIndex = checkValues[value][0];// Vì đã kiểm tra trùng trên list nội bộ nên index đầu tiên chính là index của entity đó
                        var checkEntity = entities[checkEntityIndex]; 

                        // Nếu đang cập nhật thì chỉ thêm nếu giá trị bị trùng là của record khác
                        if (checkEntity.State == ModalState.Update && id != checkEntity.GetId())
                        {
                            duplicateIndexs.Add(checkEntityIndex);
                        }

                        // Nếu đang thêm mới thì không cần quan tâm vì làm gì có record nào trong db
                        else if (checkEntity.State == ModalState.Insert)
                        {
                            duplicateIndexs.Add(checkEntityIndex);
                        }
                    }

                    // Tạo thông báo lỗi
                    var duplicateMessage = entities.Count == 1 ?
                        $"Giá trị trường {prop.PropertyInfo.GetPropDisplayName()} của {ReflectionUtil.GetEntityDisplayName<TEntity>()} đã tồn tại" :
                        $"Giá trị trường {prop.PropertyInfo.GetPropDisplayName()} của {ReflectionUtil.GetEntityDisplayName<TEntity>()} ({string.Join(", ", duplicateIndexs)}) đã tồn tại";
                    throw new BadRequestException(duplicateMessage);
                }
            }
        }


        private void AssignAuditProperties<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            var auditUserName = ClaimUtil.GetUserName(_httpContextAccessor.HttpContext?.User);
            var auditTime = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                if (entity.State == ModalState.Insert)
                {
                    entity.CreatedBy = auditUserName;
                    entity.CreatedDate = auditTime;
                    entity.ModifiedDate = auditTime;
                }
                else if (entity.State == ModalState.Update)
                {

                    entity.ModifiedBy = auditUserName;
                    entity.ModifiedDate = auditTime;
                }
            }
        }

        private async Task<List<TEntity>> GetSavedEntities<TEntity, TParentEntity>(IEnumerable<TEntity> entities, TParentEntity? parent = null) 
            where TParentEntity : BaseEntity
            where TEntity : BaseEntity
        {
            var paramIds = entities.Select(e => e.GetId());

            /// Bước 1: Build raw sql
            var param = string.Join(", ", entities.Select((_, index) => $"{{{index}}}"));
            var sql = $"SELECT * FROM {typeof(TEntity).Name} WHERE {typeof(TEntity).Name}Id IN ({param})";

            // Nếu parentId khác null => Đây là bước đệ quy cho danh sách con phụ thuộc => thêm điều kiện để
            // lấy thêm những thằng đang là con của cha để xác định thằng nào cần xóa
            if (parent != null)
            {
                sql += $" OR {typeof(TParentEntity).Name}Id = @{{{entities.Count()}}}";
                paramIds.Append(parent.GetId());
            }

            /// Bước 2: Lấy DS thực thể
            var curEntities = await _context.Set<TEntity>()
                .FromSqlRaw(sql, paramIds)
                .ToListAsync();

            return curEntities;
        }

        private async Task ValidateBeforeSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            

            
        }

        /// <summary>
        /// Hàm này thực hiện việc đánh dấu trạng thái của từng entity
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private async Task SetModalState<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            // Cập nhật tự động thêm / sửa
            foreach (var entity in entities)
            {
                if (entity.State != ModalState.None) continue;

                // Cập nhật là thêm mới nếu Id là Guid mặc định, ngược lại là cập nhật
                var id = entity.GetId();
                if (id == Guid.Empty)
                {
                    // Tự thêm Guid nếu tầng Business ko thêm
                    entity.SetValueByPropName(ReflectionUtil.GetIdPropName<TEntity>(), Guid.NewGuid());
                    entity.State = ModalState.Insert;
                }
                else
                {
                    entity.State = ModalState.Update;
                }
            }
        }

        /// <summary>
        /// Lấy IQueryable để thực hiện truy vấn với EF Core (hỗ trợ LINQ, Include)
        /// </summary>
        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
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

        /// <summary>
        /// Thay đổi trạng thái theo dõi property của entity
        /// </summary>
        public void SetChanged<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertySelector, bool isChanged) where TEntity : BaseEntity
        {
            _context.Entry(entity).Property(propertySelector).IsModified = isChanged;
        }

        /// <summary>
        /// Tự phát hiện thêm/sửa/xóa
        /// </summary>
        public async Task SaveRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            await _context.Set<TEntity>().AddRangeAsync(entities.Where(e => e.State == ModalState.Insert));
            _context.Set<TEntity>().UpdateRange(entities.Where(e => e.State == ModalState.Update));
            _context.Set<TEntity>().RemoveRange(entities.Where(e => e.State == ModalState.Delete));
            await SaveChangesAsync();
        }
    }
}

