#nullable enable
using Dapper;
using System.Linq;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.DL.Utils;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Metadata;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using System.Reflection;
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

        /// <summary>
        /// Khởi tạo BaseWriteDL với DbContext và HttpContextAccessor tương ứng
        /// </summary>
        public BaseWriteDL(DbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Thực hiện lưu danh sách thực thể tự động đệ quy làm phẳng cây thực thể,
        /// đồng bộ khóa ngoại và dọn dẹp các con mồ côi (Orphan Disposal)
        /// </summary>
        public async Task<List<TEntity>> SaveEntities<TEntity>(
            List<TEntity> entities, 
            TEntity? parent = null,
            Action<List<TEntity>>? onRollback = null) where TEntity : BaseEntity, new()
        {
            if (entities == null || entities.Count == 0)
            {
                return [];
            }

            await BeginTransactionAsync();
            try
            {
                var graph = new FlattenedEntityGraph();

                // Pha 1: Duyệt đệ quy làm phẳng cây thực thể gửi lên từ Client
                await TraverseAndFlattenIncoming(entities, parent, graph);

                // Pha 2: Quét DB phát hiện các con mồ côi cần xóa (Orphan Disposal)
                var rootUpdates = entities.Where(e => e.State == ModalState.Update).ToList();
                if (rootUpdates.Count > 0)
                {
                    await DetectOrphansForParents(rootUpdates, graph);
                }

                // Pha 3 & 4: Gom nhóm theo Type để gán Audit, Validate và đưa vào Change Tracker
                var allTypes = graph.Inserts.Keys
                    .Union(graph.Updates.Keys)
                    .Union(graph.Deletes.Keys)
                    .ToList();

                foreach (var type in allTypes)
                {
                    var inserts = graph.Inserts.GetValueOrDefault(type) ?? new();
                    var updates = graph.Updates.GetValueOrDefault(type) ?? new();
                    var deletes = graph.Deletes.GetValueOrDefault(type) ?? new();

                    var listType = typeof(List<>).MakeGenericType(type);
                    var addMethod = listType.GetMethod("Add")!;

                    // Gán Audit
                    var allInsertUpdate = inserts.Concat(updates).ToList();
                    if (allInsertUpdate.Count > 0)
                    {
                        AssignAuditProperties(allInsertUpdate);
                    }

                    // Validate hàng loạt
                    var allEntitiesOfType = inserts.Concat(updates).Concat(deletes).ToList();
                    if (allEntitiesOfType.Count > 0)
                    {
                        var typedCheckList = Activator.CreateInstance(listType)!;
                        foreach (var entity in allEntitiesOfType)
                        {
                            addMethod.Invoke(typedCheckList, new object[] { entity });
                        }

                        var checkTask = (Task)DynamicQueryUtil.InvokeGenericMethod(
                            this,
                            typeof(BaseWriteDL),
                            "CheckExist",
                            new[] { type },
                            new object[] { typedCheckList, graph },
                            BindingFlags.NonPublic | BindingFlags.Instance)!;
                        await checkTask;
                    }

                    // Đưa vào Change Tracker
                    if (inserts.Count > 0)
                    {
                        var dbSet = DynamicQueryUtil.InvokeGenericMethod(
                            _context,
                            typeof(DbContext),
                            nameof(DbContext.Set),
                            new[] { type },
                            Array.Empty<object>())!;

                        var typedInsertList = Activator.CreateInstance(listType)!;
                        foreach (var entity in inserts)
                        {
                            addMethod.Invoke(typedInsertList, new object[] { entity });
                        }

                        var dbSetType = typeof(DbSet<>).MakeGenericType(type);
                        var addRangeMethod = dbSetType.GetMethods()
                            .First(m => m.Name == nameof(DbSet<object>.AddRangeAsync) && 
                                        m.GetParameters().Length == 2 && 
                                        m.GetParameters()[0].ParameterType.IsAssignableFrom(typedInsertList.GetType()));

                        var addTask = (Task)addRangeMethod.Invoke(dbSet, new object[] { typedInsertList, default(CancellationToken) })!;
                        await addTask;
                    }

                    if (updates.Count > 0)
                    {
                        var updateIds = updates.Select(u => GetEntityId(u)).ToList();
                        var savedEntities = await GetDbChildrenDynamic(type, $"{type.Name}Id", updateIds);

                        foreach (var entity in updates)
                        {
                            var savedEntity = savedEntities.FirstOrDefault(e => GetEntityId(e) == GetEntityId(entity));
                            if (savedEntity != null)
                            {
                                _context.Entry(savedEntity).CurrentValues.SetValues(entity);
                                _context.Entry(savedEntity).Property(x => x.CreatedBy).IsModified = false;
                                _context.Entry(savedEntity).Property(x => x.CreatedDate).IsModified = false;
                            }
                        }
                    }

                    if (deletes.Count > 0)
                    {
                        var deleteIds = deletes.Select(d => GetEntityId(d)).ToList();

                        var trackedDeletes = _context.ChangeTracker.Entries()
                            .Where(e => type.IsAssignableFrom(e.Entity.GetType()) && deleteIds.Contains(GetEntityId((BaseEntity)e.Entity)))
                            .Select(e => (BaseEntity)e.Entity)
                            .ToList();

                        var missingDeleteIds = deleteIds.Except(trackedDeletes.Select(d => GetEntityId(d))).ToList();
                        if (missingDeleteIds.Count > 0)
                        {
                            var missingDeletes = await GetDbChildrenDynamic(type, $"{type.Name}Id", missingDeleteIds);
                            trackedDeletes.AddRange(missingDeletes);
                        }

                        if (trackedDeletes.Count > 0)
                        {
                            var dbSetType = typeof(DbSet<>).MakeGenericType(type);
                            var dbSet = DynamicQueryUtil.InvokeGenericMethod(
                                _context,
                                typeof(DbContext),
                                nameof(DbContext.Set),
                                new[] { type },
                                Array.Empty<object>())!;

                            var typedDeleteList = Activator.CreateInstance(listType)!;
                            foreach (var entity in trackedDeletes)
                            {
                                addMethod.Invoke(typedDeleteList, new object[] { entity });
                            }

                            var removeRangeMethod = dbSetType.GetMethod(nameof(DbSet<object>.RemoveRange), new[] { typeof(IEnumerable<>).MakeGenericType(type) })
                                ?? dbSetType.GetMethod(nameof(DbSet<object>.RemoveRange), new[] { typeof(object[]) });

                            removeRangeMethod.Invoke(dbSet, new object[] { typedDeleteList });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await CommitTransactionAsync();

                return entities;
            }
            catch
            {
                await RollbackTransactionAsync();
                onRollback?.Invoke(entities);
                throw;
            }
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
        /// Thay đổi trạng thái theo dõi property của entity
        /// </summary>
        public void SetChanged<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertySelector, bool isChanged) where TEntity : BaseEntity
        {
            _context.Entry(entity).Property(propertySelector).IsModified = isChanged;
        }

        #region Private Helper Methods

        /// <summary>
        /// CheckExist: Kiểm tra sự tồn tại hoặc không trùng lặp của các bản ghi trong cơ sở dữ liệu
        /// dựa trên các thuộc tính [CheckExist]/[Key]/[FK] đã được định nghĩa trong lớp thực thể.
        /// </summary>
        private async Task CheckExist<TEntity>(List<TEntity> entities, FlattenedEntityGraph graph)
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
                    if (entities[i].State == ModalState.None) continue;

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
                // Kiểu thực thể sẽ kiểm tra
                var targetEntityType = 
                    checkExistAttr?.TargetEntity ?? 
                    fkAttr?.TargetEntity ??
                    metadata.EntityType;

                // Bảng sẽ kiểm tra
                var targetTableName = targetEntityType.Name;

                // Tên thuộc tính sẽ kiểm tra
                var checkColumnName = prop.PropertyName;

                /// Bước 5: Kiểm tra đảm bảo giá trị tồn tại 
                if ((checkExistAttr != null && checkExistAttr.MustExist) || fkAttr != null || isFkProp)
                {
                    // Thu thập danh sách các thực thể cha đang được thêm mới trong cùng đợt lưu
                    var insertedIds = new HashSet<object>();
                    if (graph != null && graph.Inserts.TryGetValue(targetEntityType, out var inserts))
                    {
                        foreach (var ins in inserts)
                        {
                            insertedIds.Add(GetEntityId(ins));
                        }
                    }

                    // Lọc bỏ những giá trị nằm trong danh sách đang thêm mới
                    var valuesToQuery = checkValues.Keys.Where(k => !insertedIds.Contains(k)).ToList();

                    // Đối chiếu thành công ban đầu gồm các bản ghi đang thêm mới
                    var validValues = new HashSet<object>(insertedIds);

                    if (valuesToQuery.Count > 0)
                    {
                        // Xác định cột tương ứng trên bảng đích (nếu trỏ sang bảng cha thì dùng khóa chính của bảng cha, ngược lại dùng tên thuộc tính hiện tại)
                        var targetColumnName = (checkExistAttr?.TargetEntity != null || fkAttr?.TargetEntity != null) 
                            ? $"{targetTableName}Id" 
                            : checkColumnName;

                        // Câu SQL truy vấn lấy danh sách giá trị ra
                        var sql = @$"
                            SELECT {targetColumnName} FROM {targetTableName}
                            WHERE {targetColumnName} IN ({string.Join(", ", valuesToQuery.Select((_, index) => $"{{{index}}}"))})";
                        
                        var propType = prop.PropertyInfo.PropertyType;
                        var targetType = Nullable.GetUnderlyingType(propType) ?? propType;

                        var queryable = (IQueryable)DynamicQueryUtil.InvokeGenericMethod(
                            null,
                            typeof(RelationalDatabaseFacadeExtensions),
                            nameof(RelationalDatabaseFacadeExtensions.SqlQueryRaw),
                            new[] { targetType },
                            new object[] { _context.Database, sql, valuesToQuery.ToArray() })!;

                        foreach (var val in queryable)
                        {
                            if (val != null)
                            {
                                validValues.Add(val);
                            }
                        }
                    }

                    if (checkValues.Keys.All(k => validValues.Contains(k))) continue;

                    // Tạo thông báo lỗi
                    var missValues = checkValues.Keys.Where(k => !validValues.Contains(k)).ToList();
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
                    var parameters = new DynamicParameters();
                    var paramNames = new List<string>();
                    int pIndex = 0;
                    foreach (var val in checkValues.Keys)
                    {
                        var paramName = $"@p{pIndex}";
                        parameters.Add(paramName, val);
                        paramNames.Add(paramName);
                        pIndex++;
                    }

                    var sql = @$"
                        SELECT {targetTableName}Id, {checkColumnName} FROM {targetTableName}
                        WHERE {checkColumnName} IN ({string.Join(", ", paramNames)})";
                    var result = await _context.Database.GetDbConnection().QueryAsync(sql, parameters);
                    var duplicateIndexs = new List<int>();
                    foreach (var row in result)
                    {
                        var rowDict = (IDictionary<string, object>)row;
                        var value = rowDict[checkColumnName];
                        var id = Guid.Parse(rowDict[$"{targetTableName}Id"].ToString()!);
                        var checkEntityIndex = checkValues[value][0] - 1;// Đổi từ 1-indexed về 0-indexed
                        var checkEntity = entities[checkEntityIndex]; 

                        // Nếu đang cập nhật thì chỉ thêm nếu giá trị bị trùng là của record khác
                        if (checkEntity.State == ModalState.Update && id != GetEntityId(checkEntity))
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
                    if (duplicateIndexs.Count > 0)
                    {
                        var duplicateMessage = entities.Count == 1 ?
                            $"Giá trị trường {prop.PropertyInfo.GetPropDisplayName()} của {ReflectionUtil.GetEntityDisplayName<TEntity>()} đã tồn tại" :
                            $"Giá trị trường {prop.PropertyInfo.GetPropDisplayName()} của {ReflectionUtil.GetEntityDisplayName<TEntity>()} ({string.Join(", ", duplicateIndexs)}) đã tồn tại";
                        throw new BadRequestException(duplicateMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Gán các thuộc tính Audit (CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) cho danh sách thực thể
        /// dựa trên thông tin người dùng đăng nhập trong HttpContext
        /// </summary>
        /// <param name="entities">Danh sách thực thể cần gán</param>
        private void AssignAuditProperties(IEnumerable<BaseEntity> entities)
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

        /// <summary>
        /// Truy vấn dữ liệu hiện tại từ database của các thực thể để chuẩn bị so sánh cập nhật hoặc xóa
        /// </summary>
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

        /// <summary>
        /// Hàm này thực hiện việc đánh dấu trạng thái của từng entity
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private async Task SetModalState(IEnumerable<BaseEntity> entities)
        {
            // Cập nhật tự động thêm / sửa
            foreach (var entity in entities)
            {
                if (entity.State != ModalState.None) continue;

                // Cập nhật là thêm mới nếu Id là Guid mặc định, ngược lại là cập nhật
                var id = GetEntityId(entity);
                if (id == Guid.Empty)
                {
                    // Tự thêm Guid nếu tầng Business ko thêm
                    var type = entity.GetType();
                    var realType = _context.Model.FindEntityType(type)?.ClrType ?? type;
                    var idPropName = $"{realType.Name}Id";
                    entity.SetValueByPropName(idPropName, Guid.NewGuid());
                    entity.State = ModalState.Insert;
                }
                else
                {
                    entity.State = ModalState.Update;
                }
            }
        }

        private class FlattenedEntityGraph
        {
            public Dictionary<Type, List<BaseEntity>> Inserts { get; } = new();
            public Dictionary<Type, List<BaseEntity>> Updates { get; } = new();
            public Dictionary<Type, List<BaseEntity>> Deletes { get; } = new();
            public HashSet<BaseEntity> Processed { get; } = new();
        }

        /// <summary>
        /// Lấy giá trị khóa chính (Id) của thực thể dựa trên metadata cấu hình từ EF Core
        /// </summary>
        /// <param name="entity">Thực thể cần lấy Id</param>
        /// <returns>Giá trị Guid của khóa chính</returns>
        private Guid GetEntityId(BaseEntity entity)
        {
            var type = entity.GetType();
            var realType = _context.Model.FindEntityType(type)?.ClrType ?? type;
            var idPropName = $"{realType.Name}Id";
            var rawId = entity.GetValueByPropName(idPropName);
            if (rawId == null)
            {
                var backupProp = type.GetProperty("Id") ?? type.GetProperty("ID");
                if (backupProp != null)
                {
                    rawId = backupProp.GetValue(entity);
                }
            }
            if (rawId != null && Guid.TryParse(rawId.ToString(), out Guid id))
            {
                return id;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Truy vấn danh sách thực thể con từ DB một cách động sử dụng Reflection và Raw SQL để tối ưu hiệu năng
        /// </summary>
        /// <param name="childType">Kiểu dữ liệu của thực thể con</param>
        /// <param name="foreignKeyName">Tên cột khóa ngoại liên kết với cha</param>
        /// <param name="parentIds">Danh sách Id của thực thể cha</param>
        /// <returns>Danh sách thực thể con đang tồn tại trong DB</returns>
        private async Task<List<BaseEntity>> GetDbChildrenDynamic(Type childType, string foreignKeyName, List<Guid> parentIds)
        {
            if (parentIds == null || parentIds.Count == 0) return new List<BaseEntity>();

            var tableName = childType.Name;
            var sql = $"SELECT * FROM {tableName} WHERE {foreignKeyName} IN ({string.Join(",", parentIds.Select((_, i) => $"{{{i}}}"))}) AND IsDeleted = 0";

            var dbSet = DynamicQueryUtil.InvokeGenericMethod(
                _context,
                typeof(DbContext),
                nameof(DbContext.Set),
                new[] { childType },
                Array.Empty<object>())!;

            var query = DynamicQueryUtil.InvokeGenericMethod(
                null,
                typeof(RelationalQueryableExtensions),
                nameof(RelationalQueryableExtensions.FromSqlRaw),
                new[] { childType },
                new object[] { dbSet, sql, parentIds.Select(id => (object)id).ToArray() })!;

            var task = (Task)DynamicQueryUtil.InvokeGenericMethod(
                null,
                typeof(EntityFrameworkQueryableExtensions),
                nameof(EntityFrameworkQueryableExtensions.ToListAsync),
                new[] { childType },
                new object[] { query, default(CancellationToken) })!;
            await task;

            var result = task.GetType().GetProperty("Result")!.GetValue(task) as System.Collections.IEnumerable;
            return result != null ? result.Cast<BaseEntity>().ToList() : new List<BaseEntity>();
        }

        /// <summary>
        /// Pha 1: Duyệt đệ quy theo chiều sâu để làm phẳng cây thực thể được gửi lên từ client
        /// </summary>
        /// <param name="entities">Danh sách thực thể đầu vào</param>
        /// <param name="parent">Thực thể cha của danh sách hiện tại (nếu có)</param>
        /// <param name="graph">Biểu đồ chứa các thực thể đã làm phẳng phân loại theo Inserts/Updates</param>
        private async Task TraverseAndFlattenIncoming(System.Collections.IEnumerable entities, BaseEntity? parent, FlattenedEntityGraph graph)
        {
            if (entities == null) return;

            var baseList = entities.Cast<BaseEntity>().ToList();
            if (baseList.Count == 0) return;

            await SetModalState(baseList);

            foreach (var entity in baseList)
            {
                if (!graph.Processed.Add(entity)) continue;

                if (parent != null)
                {
                    var parentType = parent.GetType();
                    var parentRealType = _context.Model.FindEntityType(parentType)?.ClrType ?? parentType;
                    var parentKeyName = $"{parentRealType.Name}Id";

                    var fkProp = entity.GetType().GetProperty(parentKeyName);
                    if (fkProp != null)
                    {
                        fkProp.SetValue(entity, GetEntityId(parent));
                    }
                }

                var type = entity.GetType();
                if (entity.State == ModalState.Insert)
                {
                    if (!graph.Inserts.ContainsKey(type)) graph.Inserts[type] = new List<BaseEntity>();
                    graph.Inserts[type].Add(entity);
                }
                else if (entity.State == ModalState.Update)
                {
                    if (!graph.Updates.ContainsKey(type)) graph.Updates[type] = new List<BaseEntity>();
                    graph.Updates[type].Add(entity);
                }
                else if (entity.State == ModalState.Delete)
                {
                    if (!graph.Deletes.ContainsKey(type)) graph.Deletes[type] = new List<BaseEntity>();
                    graph.Deletes[type].Add(entity);
                }

                var childProps = type.GetProperties()
                    .Where(p => p.PropertyType.IsGenericType &&
                                typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType) &&
                                typeof(BaseEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments()[0]))
                    .ToList();

                foreach (var prop in childProps)
                {
                    var incomingChildrenRaw = prop.GetValue(entity) as System.Collections.IEnumerable;
                    if (incomingChildrenRaw == null) continue;

                    var incomingChildren = incomingChildrenRaw.Cast<BaseEntity>().ToList();
                    if (incomingChildren.Count == 0) continue;

                    await TraverseAndFlattenIncoming(incomingChildren, entity, graph);
                }
            }
        }

        /// <summary>
        /// Pha 2: Quét DB để phát hiện các con mồ côi (không nằm trong danh sách gửi lên từ client nữa) và đánh dấu trạng thái Delete
        /// </summary>
        /// <param name="parentsRaw">Danh sách các thực thể cha đang thực hiện cập nhật</param>
        /// <param name="graph">Biểu đồ chứa danh sách thực thể để đẩy các thực thể mồ côi vào Deletes</param>
        private async Task DetectOrphansForParents(System.Collections.IEnumerable parentsRaw, FlattenedEntityGraph graph)
        {
            if (parentsRaw == null) return;

            var parents = parentsRaw.Cast<BaseEntity>().ToList();
            if (parents.Count == 0) return;

            var parentType = parents[0].GetType();
            var parentRealType = _context.Model.FindEntityType(parentType)?.ClrType ?? parentType;
            var parentKeyName = $"{parentRealType.Name}Id";

            var childProps = parentRealType.GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                            typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType) &&
                            typeof(BaseEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments()[0]))
                .ToList();

            foreach (var prop in childProps)
            {
                var childType = prop.PropertyType.GetGenericArguments()[0];

                var parentIds = parents.Select(p => GetEntityId(p)).ToList();
                if (parentIds.Count == 0) continue;

                var dbChildren = await GetDbChildrenDynamic(childType, parentKeyName, parentIds);

                var childrenToDelete = new List<BaseEntity>();
                var childrenToUpdate = new List<BaseEntity>();

                foreach (var parent in parents)
                {
                    var parentId = GetEntityId(parent);
                    var parentDbChildren = dbChildren.Where(c => {
                        var fkVal = c.GetType().GetProperty(parentKeyName)?.GetValue(c);
                        return fkVal != null && (Guid)fkVal == parentId;
                    }).ToList();

                    var incomingChildrenRaw = prop.GetValue(parent) as System.Collections.IEnumerable;
                    var incomingChildren = incomingChildrenRaw != null 
                        ? incomingChildrenRaw.Cast<BaseEntity>().ToList() 
                        : new List<BaseEntity>();

                    var incomingChildIds = incomingChildren.Select(c => GetEntityId(c)).ToHashSet();

                    foreach (var dbChild in parentDbChildren)
                    {
                        if (!incomingChildIds.Contains(GetEntityId(dbChild)))
                        {
                            dbChild.State = ModalState.Delete;
                            childrenToDelete.Add(dbChild);
                        }
                        else
                        {
                            var incomingChild = incomingChildren.First(c => GetEntityId(c) == GetEntityId(dbChild));
                            childrenToUpdate.Add(incomingChild);
                        }
                    }
                }

                if (childrenToDelete.Count > 0)
                {
                    if (!graph.Deletes.ContainsKey(childType)) graph.Deletes[childType] = new List<BaseEntity>();
                    graph.Deletes[childType].AddRange(childrenToDelete);
                }

                if (childrenToUpdate.Count > 0)
                {
                    await DetectOrphansForParents(childrenToUpdate, graph);
                }
            }
        }

        #endregion
    }
}

