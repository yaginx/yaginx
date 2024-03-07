using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;

namespace AgileLabs.EfCore.PostgreSQL
{
    public class CrudRepository<T> : CrudRepository where T : class, new()
    {
        public CrudRepository(IAgileLabDbContextFactory factory, ILogger logger, IMapper mapper) : base(factory, logger, mapper)
        {
        }

        public async Task<IQueryable<T>> GetQueryAsync() => (await GetDbContextAsync()).Set<T>().AsQueryable();

        #region 异步方法
        /// <summary>
        /// 通过主键获取一个对象，如果是组合主键，必须按照主键顺序填写
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="cancellationToken"></param>
        /// <returns>如果找到则返回该表对象,否则返回null</returns>
        public virtual async Task<T> GetByIdAsync(object keyValue, CancellationToken cancellationToken = default)
        {
            object[] keyArray;
            if (keyValue.GetType().IsArray)
            {
                keyArray = (object[])keyValue;
            }
            else
            {
                keyArray = new object[] { keyValue };
            }
            return await base.GetByIdAsync<T>(keyArray, cancellationToken);
        }

        /// <summary>
        /// 根据一个表达式获取一个对象
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="isNoTracking">参数为true时关闭实体模型跟踪，关闭后查询的数据不能用于修改，提高性能</param>
        /// <param name="cancellationToken"></param>
        /// <returns>该表对象</returns>
        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> exp, bool isNoTracking = false, CancellationToken cancellationToken = default)
        {
            return await base.FirstOrDefaultAsync(exp, isNoTracking, cancellationToken);
        }

        /// <summary>
        /// 刷新单个对象
        /// </summary>
        /// <param name="t"></param>
        public async Task ReloadAsync(T t)
        {
            await base.ReloadAsync(t);
        }

        /// <summary>
        ///     根据表达式获取一个泛型对象集合，已连接数据库
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="ignoreCachedChanges">是否忽略DbContext中还未提交到数据库中的缓存值</param>
        /// <param name="isNoTracking">参数为true时关闭实体模型跟踪，关闭后查询的数据不能用于修改，提高性能</param>
        /// <param name="cancellationToken"></param>
        /// <returns>结果集</returns>
        public virtual async Task<List<T>> GetByQueryAsync(
            Expression<Func<T, bool>> exp,
            bool ignoreCachedChanges = true,
            bool isNoTracking = false,
            CancellationToken cancellationToken = default)
        {
            return await base.GetByQueryAsync(exp, ignoreCachedChanges, isNoTracking, cancellationToken);
        }

        /// <summary>
        ///     根据表达式获取一个泛型对象集合，未连接数据库，需要使用ToList方法查询数据库转换成实体对象集合
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="isNoTracking">参数为true时关闭实体模型跟踪，关闭后查询的数据不能用于修改，提高性能</param>
        /// <returns>一个待查询的结果集</returns>
        public virtual Task<IQueryable<T>> GetQueryAsync(Expression<Func<T, bool>> exp, bool isNoTracking = false)
        {
            return base.GetQueryAsync(exp, isNoTracking);
        }

        public virtual Task<int> CountAsync(Expression<Func<T, bool>> exp, CancellationToken cancellationToken = default) => base.CountAsync(exp, cancellationToken);
        public virtual Task<long> LongCountAsync(Expression<Func<T, bool>> exp, CancellationToken cancellationToken = default) => base.LongCountAsync(exp, cancellationToken);

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> exp, CancellationToken cancellationToken = default) => await base.AnyAsync(exp, cancellationToken);

        /// <summary>
        ///     插入一个对象
        /// </summary>
        /// <param name="entity">要插入的对象</param>
        public virtual async Task InsertAsync(T entity)
        {
            await base.AddAsync(entity);
        }

        public async Task BatchInsertAsync(T[] entites)
        {
            await base.BatchInsertAsync(entites);
        }

        /// <summary>
        ///     更新一个对象
        /// </summary>
        /// <param name="entity">此对象必须在当前DbContext中获取出来的对象</param>
        public virtual async Task EntryUpdateAsync(T entity)
        {
            await base.UpdateEntryAsync(entity);
        }

        public virtual Task SingleUpdateAsync(T entity)
        {
            return base.UpdateSingleAsync(entity);
        }

        /// <summary>
        ///     删除一个对象
        /// </summary>
        /// <param name="entity">此对象必须在当前DbContext中获取出来的对象</param>
        public virtual async Task DeleteAsync(T entity)
        {
            await base.DeleteAsync(entity);
        }

        /// <summary>
        ///     删除一组满足查询表达式的对象
        /// </summary>
        /// <param name="exp">删除表达式</param>
        public virtual async Task BatchDeleteAsync(Expression<Func<T, bool>> exp)
        {
            await base.BatchDeleteAsync(exp);
        }

        /// <summary>
        ///     删除一组满足查询表达式的对象
        /// </summary>
        /// <param name="exp">删除表达式</param>
        public virtual async Task DeleteAsync(Expression<Func<T, bool>> exp)
        {
            await base.DeleteAsync(exp);
        }

        /// <summary>
        ///     根据主键删除一个对象
        /// </summary>
        /// <param name="keyValue">根据主键删除</param>
        /// <param name="cancellationToken"></param>
        public virtual async Task DeleteAsync(object[] keyValue, CancellationToken cancellationToken = default)
        {
            await base.DeleteByIdAsync<T>(keyValue, cancellationToken);
        }
        public virtual async Task DeleteByIdAsync(object keyValue, CancellationToken cancellationToken = default)
        {
            await DeleteByIdAsync<T>(new[] { keyValue }, cancellationToken);
        }
        /// <summary>
        ///     传递SQL来执行获取实体，只能p0，p1
        ///     例：
        /// <![CDATA[
        /// SqlQuery<Post>("SELECT * FROM dbo.Posts WHERE Author = @userSuppliedAuthor", new{userSuppliedAuthor});
        /// SqlQuery<Post>("SELECT * FROM dbo.Posts WHERE Author = @author", new {author=userSuppliedAuthor});
        /// ]]>
        /// </summary>
        /// <param name="sql">要查询的SQL，可以带参数</param>
        /// <param name="parameter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> SqlQueryAsync(string sql, object parameter = null, CancellationToken cancellationToken = default)
        {
            return await base.SqlQueryAsync<T>(sql, parameter, cancellationToken);
        }

        public virtual async Task SqlExecuteAsync(string sql, object parameter = null, CancellationToken cancellationToken = default)
        {
            await base.SqlExecuteAsync<T>(sql, parameter, cancellationToken);
        }
        #endregion
    }
}
