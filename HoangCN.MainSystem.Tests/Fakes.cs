using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.DL.Interfaces;
using HoangCN.MainSystem.Entities;
using HoangCN.Core.BL.Interfaces;

namespace HoangCN.MainSystem.Tests
{
    public class FakeReadDL : IBaseReadDL
    {
        public List<object> QueryResults { get; set; } = new();

        private static bool MatchTableName(object obj, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return true;
            var objType = obj.GetType();
            var typeName = objType.Name;
            
            if (string.Equals(typeName, tableName, StringComparison.OrdinalIgnoreCase)) return true;
            if (typeName.Contains(tableName, StringComparison.OrdinalIgnoreCase)) return true;
            
            var baseType = objType.BaseType;
            while (baseType != null)
            {
                if (string.Equals(baseType.Name, tableName, StringComparison.OrdinalIgnoreCase)) return true;
                if (baseType.Name.Contains(tableName, StringComparison.OrdinalIgnoreCase)) return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        private static string ParseTableName(string query)
        {
            var fromIdx = query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            if (fromIdx >= 0)
            {
                var afterFrom = query.Substring(fromIdx + 6).Trim();
                var endOfTableIdx = afterFrom.IndexOfAny(new[] { ' ', '\r', '\n' });
                var tableName = endOfTableIdx >= 0 ? afterFrom.Substring(0, endOfTableIdx) : afterFrom;
                return tableName.Replace("`", "").Replace("[", "").Replace("]", "");
            }
            return string.Empty;
        }

        public Task<IEnumerable<TRow>> ExecuteQueryText<TRow>(string query, DynamicParameters? parameters = null)
        {
            var tableName = ParseTableName(query);

            // If TRow is dynamic/object, convert objects in QueryResults to dictionaries representing DapperRows
            if (typeof(TRow) == typeof(object) || typeof(TRow).Name == "DapperRow" || typeof(TRow).Name == "Object")
            {
                var dicts = new List<Dictionary<string, object?>>();
                
                // Parse column names requested in the SELECT statement
                var columns = new List<string>();
                var selectIdx = query.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase);
                var fromIdx = query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                if (selectIdx >= 0 && fromIdx > selectIdx)
                {
                    var colsStr = query.Substring(selectIdx + 7, fromIdx - (selectIdx + 7));
                    columns = colsStr.Split(',')
                        .Select(c => c.Trim().Replace("`", "").Replace("[", "").Replace("]", ""))
                        .ToList();
                }

                foreach (var obj in QueryResults)
                {
                    if (obj == null) continue;
                    if (!string.IsNullOrEmpty(tableName) && !MatchTableName(obj, tableName)) continue;

                    var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    var properties = obj.GetType().GetProperties();
                    
                    if (columns.Any())
                    {
                        foreach (var col in columns)
                        {
                            var prop = properties.FirstOrDefault(p => string.Equals(p.Name, col, StringComparison.OrdinalIgnoreCase));
                            if (prop != null)
                            {
                                dict[col] = prop.GetValue(obj);
                            }
                        }
                    }
                    else
                    {
                        foreach (var prop in properties)
                        {
                            dict[prop.Name] = prop.GetValue(obj);
                        }
                    }
                    dicts.Add(dict);
                }
                return Task.FromResult<IEnumerable<TRow>>(dicts.Cast<TRow>());
            }

            // If TRow is a value type or string/Guid, select that property from objects in QueryResults
            if (typeof(TRow).IsValueType || typeof(TRow) == typeof(string) || typeof(TRow) == typeof(Guid))
            {
                var selectIdx = query.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase);
                var fromIdx = query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                if (selectIdx >= 0 && fromIdx > selectIdx)
                {
                    var colStr = query.Substring(selectIdx + 7, fromIdx - (selectIdx + 7)).Trim()
                        .Replace("`", "").Replace("[", "").Replace("]", "");
                    
                    var list = new List<TRow>();
                    foreach (var obj in QueryResults)
                    {
                        if (obj == null) continue;
                        if (!string.IsNullOrEmpty(tableName) && !MatchTableName(obj, tableName)) continue;

                        var prop = obj.GetType().GetProperties()
                            .FirstOrDefault(p => string.Equals(p.Name, colStr, StringComparison.OrdinalIgnoreCase));
                        if (prop != null)
                        {
                            var val = prop.GetValue(obj);
                            if (val is TRow typedVal)
                            {
                                list.Add(typedVal);
                            }
                            else if (val != null)
                            {
                                try
                                {
                                    list.Add((TRow)Convert.ChangeType(val, typeof(TRow)));
                                }
                                catch
                                {
                                    // Ignore conversion errors
                                }
                            }
                        }
                    }
                    return Task.FromResult<IEnumerable<TRow>>(list);
                }
            }
            
            // Otherwise, filter by TRow type
            var results = QueryResults.OfType<TRow>().ToList();
            return Task.FromResult<IEnumerable<TRow>>(results);
        }

        public Task<TResult?> ExecuteQueryToGetFirstResult<TResult>(string query, DynamicParameters? parameters = null)
        {
            var tableName = ParseTableName(query);

            if (typeof(TResult).IsValueType || typeof(TResult) == typeof(string) || typeof(TResult) == typeof(Guid))
            {
                // Try executing query to get column list
                var selectIdx = query.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase);
                var fromIdx = query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                if (selectIdx >= 0 && fromIdx > selectIdx)
                {
                    var colStr = query.Substring(selectIdx + 7, fromIdx - (selectIdx + 7)).Trim()
                        .Replace("`", "").Replace("[", "").Replace("]", "");
                    if (string.Equals(colStr, "COUNT(*)", StringComparison.OrdinalIgnoreCase))
                    {
                        // Return total count of matching items in QueryResults
                        var count = QueryResults.Count(obj => MatchTableName(obj, tableName));
                        return Task.FromResult<TResult?>((TResult)Convert.ChangeType(count, typeof(TResult)));
                    }
                    
                    foreach (var obj in QueryResults)
                    {
                        if (obj == null) continue;
                        if (!string.IsNullOrEmpty(tableName) && !MatchTableName(obj, tableName)) continue;

                        var prop = obj.GetType().GetProperties()
                            .FirstOrDefault(p => string.Equals(p.Name, colStr, StringComparison.OrdinalIgnoreCase));
                        if (prop != null)
                        {
                            var val = prop.GetValue(obj);
                            if (val is TResult typedVal) return Task.FromResult<TResult?>(typedVal);
                            if (val != null)
                            {
                                try
                                {
                                    return Task.FromResult<TResult?>((TResult)Convert.ChangeType(val, typeof(TResult)));
                                }
                                catch {}
                            }
                        }
                    }
                }
            }

            var result = QueryResults.OfType<TResult>().FirstOrDefault();
            return Task.FromResult<TResult?>(result);
        }
    }

    public class FakeWriteDL : IBaseWriteDL
    {
        public List<object> InsertedEntities { get; } = new();
        public List<object> UpdatedEntities { get; } = new();
        public List<object> DeletedEntities { get; } = new();

        public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            return new List<TEntity>().AsQueryable();
        }

        public Task BeginTransactionAsync() => Task.CompletedTask;
        public Task CommitTransactionAsync() => Task.CompletedTask;
        public Task RollbackTransactionAsync() => Task.CompletedTask;
        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            InsertedEntities.AddRange(entities.Cast<object>());
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            UpdatedEntities.AddRange(entities.Cast<object>());
            return Task.CompletedTask;
        }

        public Task DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            DeletedEntities.AddRange(entities.Cast<object>());
            return Task.CompletedTask;
        }
    }

    public class FakeRoleBL : IBaseBL<Role>
    {
        public List<Role> Roles { get; set; } = new();

        public Task<List<TResult>> GetByCondition<TResult>(Expression<Func<Role, bool>> condition)
        {
            var func = condition.Compile();
            var matched = Roles.Where(func).Cast<TResult>().ToList();
            return Task.FromResult(matched);
        }

        public Task InsertAsync(List<Role> entities) => throw new NotImplementedException();
        public Task UpdateAsync(List<Role> entities) => throw new NotImplementedException();
        public Task DeleteAsync(DeleteRequest request) => throw new NotImplementedException();
        public Task<ResultDto<TResult>> Get<TResult>(GetRequest request) => throw new NotImplementedException();
        public Task<ResultDto<TResult>> Get<TResult>(GetRequest request, Expression<Func<Role, bool>> condition) => throw new NotImplementedException();
        public Task<TResult?> GetById<TResult>(Guid id) => throw new NotImplementedException();
    }
}
