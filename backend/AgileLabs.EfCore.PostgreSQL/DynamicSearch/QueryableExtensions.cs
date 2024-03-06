using AgileLabs.EfCore.PostgreSQL.DynamicSearch;
using AgileLabs.EfCore.PostgreSQL.DynamicSearch.Model;
using System.Diagnostics.Contracts;

namespace AgileLabs.EfCore.PostgreSQL.DynamicSearch;

public static class QueryableExtensions
{
    /// <summary>
    ///     重载IQueryable 的Where方法，以便支持QueryModel 参数
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="table">IQueryable的查询对象</param>
    /// <param name="model">QueryModel对象</param>
    /// <param name="prefix">使用前缀区分查询条件</param>
    /// <returns></returns>
    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> table, QueryModel model,
        string prefix = "") where TEntity : class
    {
        Contract.Requires(table != null);
        return Where(table, model.Items, prefix);
    }
    /// <summary>
    ///     重载IQueryable 的Where方法，以便支持QueryModel 参数
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="table">IQueryable的查询对象</param>
    /// <returns></returns>
    public static IQueryable<TEntity> DomainFilter<TEntity>(this IQueryable<TEntity> table)
    {
        Contract.Requires(table != null);
        var searchParameters = new SearchParameters();
        ForceFilterCondition.FilterByTenantDomainWearHouse<TEntity>(searchParameters);
        return Where(table, searchParameters.QueryModel.Items, "");
    }
    /// <summary>
    ///     排序
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    /// <param name="source">要排序的数据源</param>
    /// <param name="value">排序依据（加空格）排序方式</param>
    /// <returns>IOrderedQueryable</returns>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string value)
    {
        string[] arr = value.Split(' ');
        string Name = arr[1].ToUpper() == "DESC" ? "OrderByDescending" : "OrderBy";
        return ApplyOrder(source, arr[0], Name);
    }

    /// <summary>
    ///     Linq动态排序再排序
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    /// <param name="source">要排序的数据源</param>
    /// <param name="value">排序依据（加空格）排序方式</param>
    /// <returns>IOrderedQueryable</returns>
    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string value)
    {
        string[] arr = value.Split(' ');
        string Name = arr[1].ToUpper() == "DESC" ? "ThenByDescending" : "ThenBy";
        return ApplyOrder(source, arr[0], Name);
    }

    /// <summary>
    ///     排序方法
    /// </summary>
    /// <typeparam name="T">对象</typeparam>
    /// <param name="source">集合</param>
    /// <param name="property">字段名</param>
    /// <param name="methodName">排序方法</param>
    /// <returns>排过序的对象集合</returns>
    private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
    {
        Type type = typeof(T);
        ParameterExpression arg = Expression.Parameter(type, "a");
        PropertyInfo pi = type.GetProperty(property);
        Expression expr = Expression.Property(arg, pi);
        type = pi.PropertyType;
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
        object result = typeof(Queryable).GetMethods().Single(
            a => a.Name == methodName
                 && a.IsGenericMethodDefinition
                 && a.GetGenericArguments().Length == 2
                 && a.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), type)
            .Invoke(null, new object[] { source, lambda });
        return (IOrderedQueryable<T>)result;
    }

    /// <summary>
    ///     内部方法提供给Where方法调用，使用了 prefix参数过滤查询条件
    /// </summary>
    /// <typeparam name="T">实体对象</typeparam>
    /// <param name="table">集合</param>
    /// <param name="items">查询条件集合</param>
    /// <param name="prefix">属性前缀，如果使用了属性前缀则只会过滤满足属性前缀的条件集合</param>
    /// <returns>对象集合</returns>
    private static IQueryable<T> Where<T>(IQueryable<T> table, IEnumerable<ConditionItem> items, string prefix = "")
    {
        Contract.Requires(table != null);
        IEnumerable<ConditionItem> filterItems =
            string.IsNullOrWhiteSpace(prefix)
                ? items.Where(c => string.IsNullOrEmpty(c.Prefix))
                : items.Where(c => c.Prefix == prefix);
        if (filterItems.Count() == 0) return table;
        return new QueryableSearcher<T>(table, filterItems).Search();
    }

    public static async Task<int> SafeCountAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        return query is IAsyncEnumerable<T> ? await query.CountAsync(cancellationToken) : query.Count();
    }

    public static async Task<List<T>> SafeToListAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        return query is IAsyncEnumerable<T> ? await query.ToListAsync(cancellationToken) : query.ToList();
    }
    #region 分页处理

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="query">待查询的结果集</param>
    /// <param name="pageInfo">分页信息<see cref="PageInfo" /></param>
    /// <param name="count">总记录数</param>
    /// <returns>一个待查询的结果集</returns>
    public static IQueryable<T> GetByPage<T>(this IQueryable<T> query, PageInfo pageInfo, out int count) where T : class
    {
        return Pagination<T>.PageList(
            query,
            pageInfo.SkipCount > 0 ? pageInfo.SkipCount : (pageInfo.CurrentPage - 1) * pageInfo.PageSize,
            pageInfo.PageSize,
            pageInfo.SortField,
            pageInfo.SortDirection,
            out count,
            pageInfo.IsGetTotalCount);
    }

    /// <summary>
    /// 分页查询
    /// 如果T中有OraModel，并且查询总记录数，并且查询条件都隶属于OraModel，则使用单表查询
    /// </summary>
    /// <param name="searchParameters">查询参数 <see cref="SearchParameters" /></param>
    /// <param name="sqlwhere">字符串的查询条件</param>
    /// <returns>一个待查询的结果集</returns>
    public static IQueryable<T> GetByPage<T>(this IQueryable<T> query, SearchParameters searchParameters, string sqlwhere = "") where T : class
    {
        searchParameters.RebuildSearchParamForOracleInClause();

        //处理EF查询时，查询null值问题
        searchParameters.BuildEmptySearch();

        IQueryable<T> list = query.Where(searchParameters.QueryModel);
        if (!string.IsNullOrEmpty(sqlwhere))
        {
            list = list.Where(sqlwhere);
        }

        var result = list.GetByPage(searchParameters.PageInfo, out int totalCount);
        searchParameters.PageInfo.TotalCount = totalCount;
        return result;
    }

    /// <summary>
    /// 分页查询
    /// 如果T中有OraModel，并且查询总记录数，并且查询条件都隶属于OraModel，则使用单表查询
    /// </summary>
    /// <param name="searchParameters">查询参数 <see cref="SearchParameters" /></param>
    /// <param name="sqlwhere">字符串的查询条件</param>
    /// <returns>一个待查询的结果集</returns>
    public static IQueryable<T> GetByPage<T>(this IQueryable<T> query, SearchParameters searchParameters, Func<IQueryable<T>, IQueryable<T>> andWhere, string sqlwhere = "") where T : class
    {
        searchParameters.RebuildSearchParamForOracleInClause();

        //处理EF查询时，查询null值问题
        searchParameters.BuildEmptySearch();
        IQueryable<T> list = query.Where(searchParameters.QueryModel);
        if (!string.IsNullOrEmpty(sqlwhere))
        {
            list = list.Where(sqlwhere);
        }

        if (andWhere != null)
            list = andWhere(list);

        var result = list.GetByPage(searchParameters.PageInfo, out int totalCount);
        searchParameters.PageInfo.TotalCount = totalCount;
        return result;
    }

    #endregion

    /// <summary>
    /// 重建查询参数
    /// </summary>
    /// <param name="searchParameters"></param>
    public static void RebuildSearchParamForOracleInClause(this SearchParameters searchParameters)
    {
        int oracleInCount = 1000;
        var stdInItems = new List<ConditionItem>();
        //找数组元素超过1000
        foreach (var item in searchParameters.QueryModel.Items)
        {
            if (item.Method == QueryMethod.StdIn)
            {
                if (item.Value is string[])
                {
                    var valueArray = (string[])item.Value;
                    //数组元素大于1000需要处理
                    if (valueArray != null && valueArray.Length > oracleInCount)
                    {
                        stdInItems.Add(item);
                    }
                }
                else if (item.Value is List<string>)
                {
                    var valueArray = ((List<string>)item.Value).ToArray();
                    if (valueArray != null && valueArray.Length > oracleInCount)
                    {
                        stdInItems.Add(item);
                    }
                }
                else if (item.Value is int[])
                {
                    var valueArray = (int[])item.Value;
                    if (valueArray != null && valueArray.Length > oracleInCount)
                    {
                        stdInItems.Add(item);
                    }
                }
                else if (item.Value is List<int>)
                {
                    var valueArray = ((List<int>)item.Value).ToArray();
                    if (valueArray != null && valueArray.Length > oracleInCount)
                    {
                        stdInItems.Add(item);
                    }
                }
            }
        }
        foreach (var item in stdInItems)
        {
            //OrGroup为空时设置值
            if (string.IsNullOrEmpty(item.OrGroup))
            {
                item.OrGroup = $"{item.Field}OracleInGroup";
            }
            if (item.Value is string[])
            {
                var valueArray = (string[])item.Value;
                if (valueArray != null)
                {
                    RebuildSearchParamForOracleInClause(ref searchParameters, item, valueArray, oracleInCount);
                }
            }
            else if (item.Value is List<string>)
            {
                var valueArray = ((List<string>)item.Value).ToArray();
                if (valueArray != null)
                {
                    RebuildSearchParamForOracleInClause(ref searchParameters, item, valueArray, oracleInCount);
                }
            }
            else if (item.Value is int[])
            {
                var valueArray = (int[])item.Value;
                if (valueArray != null)
                {
                    RebuildSearchParamForOracleInClause(ref searchParameters, item, valueArray, oracleInCount);
                }
            }
            else if (item.Value is List<int>)
            {
                var valueArray = ((List<int>)item.Value).ToArray();
                if (valueArray != null)
                {
                    RebuildSearchParamForOracleInClause(ref searchParameters, item, valueArray, oracleInCount);
                }
            }
        }
    }

    /// <summary>
    /// 重建查询参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="searchParameters"></param>
    /// <param name="item"></param>
    /// <param name="valueArray"></param>
    /// <param name="oracleInCount"></param>
    private static void RebuildSearchParamForOracleInClause<T>(ref SearchParameters searchParameters, ConditionItem item, T[] valueArray, int oracleInCount)
    {
        //计算in子句按1000分割数
        var inGroups = valueArray.Length / oracleInCount + (valueArray.Length % oracleInCount != 0 ? 1 : 0);
        //构造参数
        for (int i = 0; i < inGroups; i++)
        {
            T[] inGroup;
            if (i == inGroups - 1)
            {
                inGroup = valueArray.Skip(i * oracleInCount).Take(valueArray.Length - i * oracleInCount).ToArray();
            }
            else
            {
                inGroup = valueArray.Skip(i * oracleInCount).Take(oracleInCount).ToArray();
            }
            searchParameters.QueryModel.Items.Add(new ConditionItem { Field = item.Field, Method = item.Method, Value = inGroup, OrGroup = item.OrGroup, Prefix = item.Prefix });
        }
        //移除原参数项
        searchParameters.QueryModel.Items.Remove(item);
    }
}
