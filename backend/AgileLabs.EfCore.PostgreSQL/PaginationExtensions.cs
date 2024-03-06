using AgileLabs.EfCore.PostgreSQL.DynamicSearch;
using AgileLabs.EfCore.PostgreSQL.DynamicSearch.Model;

namespace AgileLabs.EfCore.PostgreSQL
{
    public static class PaginationExtensions
    {
        static Type[] NotNullType = { typeof(string), typeof(int), typeof(bool), typeof(double), typeof(Guid), typeof(decimal), typeof(DateTime) };
        /// <summary>
        /// 分页，就是将已经整理的查询条件 IQueryable 再加上 skip，take 方法
        /// </summary>
        /// <param name="query"></param>
        /// <param name="skipCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort">排序字段,如果是多字段排序,此字段记录为 "ID ASC,Name Desc" </param>
        /// <param name="dir">ASC/DESC,如果是多字段排序,此字段为空</param>
        /// <param name="isGetTotalCount">是否获取总页数，默认是true</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<PaginationQueryWarapper<T>> PageListAsync<T>(this IQueryable<T> query, int skipCount, int pageSize, string sort, string dir, bool isGetTotalCount = true, CancellationToken cancellationToken = default)
        {
            int count = 0;
            //如果是获取总记录数，则不需要排序
            if (isGetTotalCount)
            {
                try
                {
                    count = await query.SafeCountAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    //LogEx.LogError(e);
                    count = 100000;
                    throw;
                }
            }
            else
            {
                count = 100000;
            }
            //pageSize不能小于0；
            if (pageSize <= 0)
            {
                return new PaginationQueryWarapper<T>(count);
            }
            //没有排序，并且不分页 
            if (string.IsNullOrEmpty(dir) && string.IsNullOrEmpty(sort) && skipCount <= 0)
            {
                //为了提高性能，如果只查询第一页的数据，就没有Skip，可以不需要排序
                query = query.Take(pageSize);
            }
            else
            {
                IOrderedQueryable<T> iOrderedQueryable = query.OrderedQueryable(sort, dir, skipCount);
                query = iOrderedQueryable.Skip(skipCount).Take(pageSize);
            }

            return new PaginationQueryWarapper<T>(query, count);
        }

        /// <summary>
        /// 构建排序
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <param name="dir"></param>
        /// <param name="skipCount"></param>
        /// <returns></returns>
        private static IOrderedQueryable<T> OrderedQueryable<T>(this IQueryable<T> query, string sort, string dir, int skipCount)
        {
            IOrderedQueryable<T> iOrderedQueryable = null;
            if (string.IsNullOrEmpty(dir))
            {
                if (!string.IsNullOrEmpty(sort))
                {
                    //如果是多字段排序，则会把排序字段和排序方式记录到sort上，dir为空
                    char[] delimiters = { ',' };
                    string[] sorts = sort.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < sorts.Length; i++)
                    {
                        string[] values = sorts[i].Trim().Split(' ');
                        string sortName = values[0];
                        string sortDir = values.Length == 2 ? values[1] : "ASC";
                        iOrderedQueryable = i == 0
                            ? query.OrderBy(sortName + " " + sortDir)
                            : iOrderedQueryable.ThenBy(sortName + " " + sortDir);
                    }
                }
                else if (skipCount > 0)
                {
                    //如果没有传递任何排序字段,则按第一个字段排序.
                    //此处会引起数据不准确. 需要发送告警邮件和日志
                    // ExceptionEx.ExceptionTool.CreateWarning($"{typeof(T).FullName}分页查询未指定排序字段,可能会导致数据查询不准确,请及时处理");
                    iOrderedQueryable = query.OrderBy(ReflectionTool.GetPropertyInfosFromCache(typeof(T))[0].Name + " ASC");
                }
            }
            else
            {
                iOrderedQueryable = query.OrderBy(sort + " " + dir);
            }
            return iOrderedQueryable;
        }

        /// <summary>
        /// 对List进行分页, 此方法会强制大小写匹配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchParameters">查询条件</param>
        /// <param name="cancellationToken"></param>
        /// <param name="list">泛型集合</param>
        /// <returns></returns>
        public static async Task<List<T>> PagingListDataAsync<T>(this List<T> list, SearchParameters searchParameters, CancellationToken cancellationToken = default) where T : class
        {
            PageInfo page = searchParameters.PageInfo;
            // 这里需要转换一下查询条件, 将不等于null的判断加入到QueryModel中,否则会抛出NullReferenceException异常
            var notNullConditions = new ConditionItem[searchParameters.QueryModel.Items.Count];
            // 只复制 nullable 字段, 如果本身就是not null类型，添加了过滤条件会报错
            var nullablePropertyNames = typeof(T).GetProperties().Where(a => !NotNullType.Contains(a.PropertyType)).Select(a => a.Name).ToList();
            searchParameters.QueryModel.Items.Where(a => nullablePropertyNames.Contains(a.Field)).ToList().CopyTo(notNullConditions);
            foreach (ConditionItem conditionItem in notNullConditions)
            {
                if (conditionItem != null)
                {
                    var temp = new ConditionItem { Field = conditionItem.Field, Method = QueryMethod.NotEqual, Value = null };
                    searchParameters.QueryModel.Items.Insert(0, temp);
                }
            }
            var result = await list.AsQueryable().Where(searchParameters.QueryModel).PageListAsync(
                    page.SkipCount == 0 ? page.GetSkipCountByPageIndex() : page.SkipCount,
                    page.PageSize,
                    page.SortField,
                    page.SortDirection, cancellationToken: cancellationToken);
            var srclist = await result.ToListAsync(cancellationToken);
            page.TotalCount = result.Count;
            return srclist;
        }
    }}
