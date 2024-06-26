﻿using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AgileLabs.EfCore.PostgreSQL.DynamicSearch;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Polly;

namespace AgileLabs.EfCore.PostgreSQL
{
    public class CrudRepository
    {
        private readonly IAgileLabDbContextFactory _dbContextFactory;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        protected readonly int MaxQueryDataCount = 99999;
        public async Task<DbContext> GetDbContextAsync() => await _dbContextFactory.GetDefaultDbContextAsync();

        public CrudRepository(IAgileLabDbContextFactory dbContextFactory, ILogger logger, IMapper mapper)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
        }

        #region 异步方法

        /// <summary>
        /// 通过主键获取一个对象，如果是组合主键，必须按照主键顺序填写
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="cancellationToken"></param>
        /// <returns>如果找到则返回该表对象,否则返回null</returns>
        public virtual async Task<T> GetByIdAsync<T>(object keyValue, CancellationToken cancellationToken = default) where T : class, new()
        {
            object[] keyArray;
            if (keyValue.GetType().IsArray)
            {
                keyArray = (object[])keyValue;
            }
            else
            {
                keyArray = [keyValue];
            }
            var typedKeies = await ProcessPrimaryKeyTypesAsync<T>(keyArray);
            var Context = await GetDbContextAsync();
            return await Context.Set<T>().FindAsync(typedKeies);
        }

        /// <summary>
        /// 根据一个表达式获取一个对象
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="isNoTracking">参数为true时关闭实体模型跟踪，关闭后查询的数据不能用于修改，提高性能</param>
        /// <param name="cancellationToken"></param>
        /// <returns>该表对象</returns>
        public virtual async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> exp, bool isNoTracking = false, CancellationToken cancellationToken = default) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            if (isNoTracking)
                return await Context.Set<T>().AsNoTracking().Where(exp).FirstOrDefaultAsync();
            else
                return await Context.Set<T>().Where(exp).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 从数据库中刷新所有已经查询出来的实体对象.
        /// </summary>
        public virtual async Task ReloadAllAsync()
        {
            var Context = await GetDbContextAsync();
            var reloadEntitys = Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Unchanged).ToList();
            foreach (var entity in reloadEntitys)
            {
                await entity.ReloadAsync();
            }
        }

        /// <summary>
        /// 刷新单个对象
        /// </summary>
        /// <param name="t"></param>
        public virtual async Task ReloadAsync<T>(T t) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            await Context.Entry(t).ReloadAsync();
        }

        /// <summary>
        /// 根据表达式获取一个泛型对象集合，已连接数据库
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="ignoreCachedChanges">是否忽略DbContext中还未提交到数据库中的缓存值</param>
        /// <param name="isNoTracking">参数为true时关闭实体模型跟踪，关闭后查询的数据不能用于修改，提高性能</param>
        /// <param name="cancellationToken"></param>
        /// <returns>结果集</returns>
        public virtual async Task<List<T>> GetByQueryAsync<T>(Expression<Func<T, bool>> exp, bool ignoreCachedChanges = true, bool isNoTracking = false, CancellationToken cancellationToken = default) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            //强制控制返回数据库行数必须小于10万条记录，如果查询行数等于10万则抛出异常，避免系统不可控
            var query = new List<T>();
            if (isNoTracking)
            {
                query = await Context.Set<T>().AsNoTracking().Where(exp).Skip(0).Take(MaxQueryDataCount).ToListAsync(cancellationToken);
            }
            else
            {
                query = await Context.Set<T>().Where(exp).Skip(0).Take(MaxQueryDataCount).ToListAsync(cancellationToken);
            }
            if (query.Count == MaxQueryDataCount)
            {
                //如果PageSize 设置了9.9万, 而且查出来的数据也等于9.9万, 则认为查询超出了程序最大范围.
                throw new Exception($"{typeof(T).FullName}查询记录数超过了9.9万行, 系统将会出现Bug, 请立即排查");
            }
            if (!ignoreCachedChanges)
            {
                var newQuery = query.AsQueryable().Except(Context.ChangeTracker.Entries<T>().Where(x => x.State == EntityState.Deleted).Select(x => x.Entity));
                newQuery = newQuery.Union(Context.ChangeTracker.Entries<T>().Where(x => x.State == EntityState.Added).Select(x => x.Entity));
                query = newQuery.ToList();
            }

            return query;
        }

        /// <summary>
        /// 根据表达式获取一个泛型对象集合，未连接数据库，需要使用ToList方法查询数据库转换成实体对象集合
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="isNoTracking">参数为true时关闭实体模型跟踪，关闭后查询的数据不能用于修改，提高性能</param>
        /// <returns>一个待查询的结果集</returns>
        public virtual async Task<IQueryable<T>> GetQueryAsync<T>(Expression<Func<T, bool>> exp, bool isNoTracking = false) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            if (isNoTracking)
                return Context.Set<T>().AsNoTracking().Where(exp);
            else
                return Context.Set<T>().Where(exp);
        }

        public virtual async Task<int> CountAsync<T>(Expression<Func<T, bool>> exp, CancellationToken cancellationToken = default) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            return await Context.Set<T>().Where(exp).CountAsync(cancellationToken);
        }

        public virtual async Task<long> LongCountAsync<T>(Expression<Func<T, bool>> exp, CancellationToken cancellationToken = default) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            return await Context.Set<T>().Where(exp).LongCountAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> exp, CancellationToken cancellationToken = default) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            return await Context.Set<T>().Where(exp).AnyAsync(cancellationToken);
        }

        /// <summary>
        /// 插入一个对象
        /// </summary>
        /// <param name="entity">要插入的对象</param>
        public virtual async Task AddAsync<T>(T entity) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            await Context.Set<T>().AddAsync(entity);
        }

        public async Task BatchInsertAsync<T>(T[] entites) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            await Context.Set<T>().AddRangeAsync(entites);
        }

        /// <summary>
        /// 更新一个对象
        /// </summary>
        /// <param name="entity">此对象必须在当前DbContext中获取出来的对象</param>
        public virtual async Task UpdateEntryAsync<T>(T entity) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            if (Context.ChangeTracker.AutoDetectChangesEnabled)
            {
                // 先获取映射实体类型定义
                IEntityType mEntityType = Context.Model.FindEntityType(typeof(T));
                // 获取主键定义
                IKey mPrimaryKeys = mEntityType.FindPrimaryKey();
                // 获取主键定义的长度
                int mKeyLen = mPrimaryKeys?.Properties?.Count ?? 0;
                if (mKeyLen == 1)
                {
                    var pkProperty = mPrimaryKeys.Properties.First();
                    var pkValue = pkProperty.PropertyInfo.GetValue(entity);

                    var attachedEntity = Context.Set<T>().Find(pkValue);
                    if (attachedEntity != null)
                    {
                        //_mapper.Map(entity, attachedEntity);

                        Context.Entry(attachedEntity).CurrentValues.SetValues(entity);
                        //// 遍历所有的属性，检查是否有变化
                        //foreach (var property in Context.Entry(attachedEntity).Properties)
                        //{
                        //    var original = property.OriginalValue;
                        //    var current = property.CurrentValue;

                        //    // 如果原始值和当前值不同，则将该属性标记为已修改
                        //    if (!object.Equals(original, current))
                        //    {
                        //        property.IsModified = true;
                        //    }
                        //    else
                        //    {
                        //        // 如果值未发生变化，确保不标记为已修改
                        //        property.IsModified = false;
                        //    }
                        //}
                    }
                }
            }
            else
            {
                //有实体跟踪,这里就不需要再Attach了, 否则就是全字段更新, 并且会更新TS,引起并发问题混乱
                Context.Set<T>().Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual async Task UpdateSingleAsync<T>(T entity) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            await Context.Set<T>().SingleUpdateAsync(entity);
        }

        /// <summary>
        ///     删除一组满足查询表达式的对象
        /// </summary>
        /// <param name="exp">删除表达式</param>
        public virtual async Task BatchDeleteAsync<T>(Expression<Func<T, bool>> exp) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            IEnumerable<T> objects = Context.Set<T>().Where(exp).ToList();
            Context.Set<T>().RemoveRange(objects);
        }
        /// <summary>
        ///     删除一组满足查询表达式的对象
        /// </summary>
        /// <param name="exp">删除表达式</param>
        public virtual async Task DeleteAsync<T>(Expression<Func<T, bool>> exp) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            //System.Reflection.Assembly.GetAssembly().CreateInstance("")
            IEnumerable<T> objects = Context.Set<T>().Where(exp).AsEnumerable();
            Context.Set<T>().RemoveRange(objects);
        }

        /// <summary>
        ///     删除一个对象
        /// </summary>
        /// <param name="entity">此对象必须在当前DbContext中获取出来的对象</param>
        public virtual async Task DeleteAsync<T>(T entity) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            Context.Set<T>().Remove(entity);
        }

        /// <summary>
        ///     根据主键删除一个对象
        /// </summary>
        /// <param name="keyValue">根据主键删除</param>
        /// <param name="cancellationToken"></param>

        public virtual async Task DeleteByIdAsync<T>(object keyValue, CancellationToken cancellationToken = default) where T : class, new()
        {
            await DeleteByIdAsync<T>(new[] { keyValue }, cancellationToken);
        }
        /// <summary>
        ///     根据主键删除一个对象
        /// </summary>
        /// <param name="keyValue">根据主键删除</param>
        /// <param name="cancellationToken"></param>

        public virtual async Task DeleteByIdAsync<T>(object[] keyValue, CancellationToken cancellationToken = default) where T : class, new()
        {
            keyValue = await ProcessPrimaryKeyTypesAsync<T>(keyValue);

            var Context = await GetDbContextAsync();
            T t = await Context.Set<T>().FindAsync(keyValue, cancellationToken);
            if (t != null)
            {
                Context.Set<T>().Remove(t);
            }
        }

        /// <summary>
        /// 传递SQL来执行获取实体
        /// 例：
        /// <![CDATA[
        /// SqlQuery<Post>("SELECT * FROM dbo.Posts WHERE Author = @userSuppliedAuthor", new{userSuppliedAuthor});
        /// SqlQuery<Post>("SELECT * FROM dbo.Posts WHERE Author = @author", new {author=userSuppliedAuthor});
        /// ]]>
        /// </summary>
        /// <param name="sql">要查询的SQL，可以带参数</param>
        /// <param name="parameter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> SqlQueryAsync<T>(string sql, object parameter = null, CancellationToken cancellationToken = default) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            //EF Core 不支持 SqlQuery，使用 Dapper代替
            var connection = Context.Database.GetDbConnection();
            var dbTransaction = Context.Database.CurrentTransaction?.GetDbTransaction();
            var executeSql = sql;
            var query = await connection.QueryAsync<T>(executeSql, parameter, transaction: dbTransaction);
            return query.ToList();
        }

        public virtual async Task SqlExecuteAsync<T>(string sql, object parameter = null, CancellationToken cancellationToken = default) where T : class, new()
        {
            var Context = await GetDbContextAsync();
            var connection = Context.Database.GetDbConnection();
            var dbTransaction = Context.Database.CurrentTransaction?.GetDbTransaction();
            var executeSql = sql;
            await connection.ExecuteAsync(executeSql, parameter, transaction: dbTransaction);
        }
        #endregion

        #region Utils 
        /// <summary>
        /// 处理主键数据类型
        /// 
        /// 只处理传入数量大于1，并且与主键数量相等的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        protected async Task<object[]> ProcessPrimaryKeyTypesAsync<T>(object[] keyValue)
        {
            if (keyValue.Length > 1)
            {
                // 先获取映射实体类型定义
                var Context = await GetDbContextAsync();
                IEntityType mEntityType = Context.Model.FindEntityType(typeof(T));
                // 获取主键定义
                IKey mPrimaryKeys = mEntityType.FindPrimaryKey();
                // 获取主键定义的长度
                int mKeyLen = mPrimaryKeys?.Properties?.Count ?? 0;
                // 查询到主键
                // 主键数量与传入参数数量相同
                if (mKeyLen == keyValue.Length)
                {
                    for (int i = 0; i < mKeyLen; i++)
                    {
                        // 如果传入的参数类型与定义类型不符，则转换为定义类型
                        if (keyValue[i] != null && keyValue[i].GetType() != mPrimaryKeys.Properties[i].PropertyInfo.PropertyType)
                            keyValue[i] = Convert.ChangeType(keyValue[i], mPrimaryKeys.Properties[i].PropertyInfo.PropertyType);
                    }
                }
            }
            return keyValue;
        }
        #endregion

        #region SearchParameters
        /// <summary>
        /// 把SearchParameters转为IQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchParameters"></param>
        /// <param name="sqlwhere"></param>
        /// <param name="isNoTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> GetSearchParameterQueryAsync<T>(SearchParameters searchParameters, string sqlwhere = "", bool isNoTracking = false, CancellationToken cancellationToken = default)
             where T : class, new()
        {
            CheckSearchParameterLength(searchParameters, sqlwhere);
            var Context = await GetDbContextAsync();
            IQueryable<T> query;
            if (isNoTracking)
            {
                query = Context.Set<T>().AsNoTracking().Where(searchParameters.QueryModel);
            }
            else
            {
                query = Context.Set<T>().Where(searchParameters.QueryModel);
            }
            if (!string.IsNullOrEmpty(sqlwhere))
            {
                query = query.Where(sqlwhere);
            }
            return query;
        }

        /// <summary>
        /// searchParameter 的Condition Item 不能超过500.
        /// sql where 的length 长度不能超过1000.
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <param name="sqlwhere"></param>
        public static void CheckSearchParameterLength(SearchParameters searchParameters, string sqlwhere)
        {
            if (searchParameters?.QueryModel?.Items?.Count > 500)
            {
                //LogEx.LogError("警告: SearchParameter 长度大于500,Lamda 表达式可能会堆栈溢出:" + searchParameters.ToJson());
            }
            if (sqlwhere != null && sqlwhere.Length > 1000)
            {
                //LogEx.LogError("警告: sqlwhere 长度大于1000,Lamda 表达式可能会堆栈溢出:" + sqlwhere.Substring(0, 500));
            }
        }

        /// <summary>
        /// 基于SearchParameters的Page分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchParameters"></param>
        /// <param name="sqlwhere"></param>
        /// <param name="isNoTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Page<T>> GetByPageAsync<T>(SearchParameters searchParameters, string sqlwhere = "", bool isNoTracking = false, CancellationToken cancellationToken = default)
         where T : class, new()
        {
            try
            {
                searchParameters.PageInfo.IsGetTotalCount = true;
                var query = await GetSearchParameterQueryAsync<T>(searchParameters, sqlwhere, isNoTracking, cancellationToken);
                var result = await GetByPageAsync(query, searchParameters.PageInfo, cancellationToken);
                return new Page<T>
                {
                    Records = await result.Query.ToListAsync(),
                    Paging = new Paging
                    {
                        PageIndex = searchParameters.PageInfo.CurrentPage,
                        Total = result.Count,
                        PageSize = searchParameters.PageInfo.PageSize
                    }
                };
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "GetByPageAsync DbException");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query">待查询的结果集</param>
        /// <param name="pageInfo">分页信息<see cref="PageInfo" /></param>
        /// <param name="cancellationToken"></param>
        /// <returns>一个待查询的结果集</returns>
        public virtual Task<PaginationQueryWarapper<T>> GetByPageAsync<T>(
            IQueryable<T> query,
            PageInfo pageInfo,
            CancellationToken cancellationToken = default) where T : class, new()
        {
            return query.PageListAsync(pageInfo.SkipCount > 0 ? pageInfo.SkipCount : pageInfo.GetSkipCountByPageIndex(), pageInfo.PageSize,
                pageInfo.SortField, pageInfo.SortDirection, pageInfo.IsGetTotalCount, cancellationToken);
        }
        #endregion
    }
}
