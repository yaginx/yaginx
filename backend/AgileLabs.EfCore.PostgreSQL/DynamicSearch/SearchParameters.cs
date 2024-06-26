using AgileLabs.EfCore.PostgreSQL.DynamicSearch.Model;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;

namespace AgileLabs.EfCore.PostgreSQL.DynamicSearch;


/// <summary>
///     查询参数，用来动态构建查询表达式树的实体
///     add by Gavin 2014-3-27
/// </summary>
[Serializable]
[DataContract]
[KnownType(typeof(string[]))]
[KnownType(typeof(int[]))]
public class SearchParameters
{
    /// <summary>
    ///     构造一个空的查询条件
    /// </summary>
    public SearchParameters()
    {
        QueryModel = new QueryModel();
        PageInfo = new PageInfo
        {
            CurrentPage = 1,
            IsPaging = true,
            PageSize = 20,
            SortDirection = "",
            SortField = "",
        };
    }

    /// <summary>
    /// 具体的查询条件
    /// </summary>
    [DataMember]
    public QueryModel QueryModel { get; set; }

    /// <summary>
    ///     分页参数
    /// </summary>
    [DataMember]
    public PageInfo PageInfo { get; set; }

    /// <summary>
    ///     根据JqGrid 的过滤参数转换成查询条件枚举
    ///     [{ oper:'eq', text:'equal'},{ oper:'ne', text:'not equal'},{ oper:'lt', text:'less'},
    ///     { oper:'le', text:'less or equal'},{ oper:'gt', text:'greater'},{ oper:'ge', text:'greater or equal'},
    ///     { oper:'bw', text:'begins with'},{ oper:'bn', text:'does not begin with'},{ oper:'in', text:'is in'},
    ///     { oper:'ni', text:'is not in'},{ oper:'ew', text:'ends with'},{ oper:'en', text:'does not end with'},
    ///     { oper:'cn', text:'contains'},{ oper:'nc', text:'does not contain'}]
    /// 补充
    ///      { oper:'mcn', text:'contains'},{ oper:'mcn', text:'more like'}
    /// </summary>
    /// <param name="op"></param>
    /// <returns></returns>
    private QueryMethod GetMethodByJqGridOp(string op)
    {
        switch (op)
        {
            case "eq":
                return QueryMethod.Equal;

            case "ne":
                return QueryMethod.NotEqual;

            case "lt":
                return QueryMethod.LessThan;

            case "le":
                return QueryMethod.LessThanOrEqual;

            case "gt":
                return QueryMethod.GreaterThan;

            case "ge":
                return QueryMethod.GreaterThanOrEqual;

            case "bw":
                return QueryMethod.StartsWith;

            case "in":
                return QueryMethod.StdIn;

            case "ni":
                return QueryMethod.StdNotIn;

            case "ew":
                return QueryMethod.EndsWith;

            case "cn":
                return QueryMethod.Contains;

            case "nc":
                return QueryMethod.NotLike;

            default:
                throw new WoScmBizException("This query condition is not supported");
        }
    }

    /// <summary>
    /// 根据QueryModel组织sqlWhere语句,如果有字段前缀的话,需要提前增加进来
    /// </summary>
    /// <returns></returns>
    public string GetSqlWhere()
    {
        var sb = new StringBuilder();
        List<string> groups = new List<string>();
        foreach (var conditionItem in QueryModel.Items)
        {
            string sqlWhere = string.Empty;
            if (!string.IsNullOrEmpty(conditionItem.OrGroup))
            {
                if (!groups.Contains(conditionItem.OrGroup))
                {
                    var sbChild = new StringBuilder();
                    foreach (var senItem in QueryModel.Items)
                    {
                        if (senItem.OrGroup == conditionItem.OrGroup)
                        {
                            if (sbChild.Length > 0)
                            {
                                sbChild.Append(" or ");
                            }
                            sbChild.Append((string.IsNullOrEmpty(senItem.Prefix) ? "" : senItem.Prefix + ".") + senItem.Field + " " + ConvertMethodToSql(senItem.Method, senItem.Value));
                        }
                    }
                    if (sb.Length > 0)
                        sb.Append(" and ");
                    sb.Append("(" + sbChild.ToString() + ")");
                    groups.Add(conditionItem.OrGroup);
                }
            }
            else
            {
                if (sb.Length > 0)
                    sb.Append(" and ");
                sb.Append((string.IsNullOrEmpty(conditionItem.Prefix) ? "" : conditionItem.Prefix + ".") + conditionItem.Field + " " + ConvertMethodToSql(conditionItem.Method, conditionItem.Value));
            }
        }
        return sb.ToString();
    }
    /// <summary>
    /// 获取QueryModel的参数列表
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetParameters()
    {
        var paraDic = new Dictionary<string, object>();
        for (int i = 0; i < QueryModel.Items.Count; i++)
        {
            var item = QueryModel.Items[i];
            switch (item.Method)
            {
                /*
                  *   sqlText += " and Name like @Name";
                        p.Add("Name","%"+ model.Name+"%");
                  */
                case QueryMethod.Contains:
                case QueryMethod.NotLike:
                    paraDic.Add(item.Field + i, "%" + item.Value + "%");
                    break;

                case QueryMethod.StartsWith:
                    paraDic.Add(item.Field + i, item.Value + "%");
                    break;

                case QueryMethod.EndsWith:
                    paraDic.Add(item.Field + i, "%" + item.Value);
                    break;
                /*
                 * string sql = "SELECT * FROM SomeTable WHERE id IN @ids"
                    var results = conn.Query(sql, new { ids = new[] { 1, 2, 3, 4, 5 }});
                 */
                case QueryMethod.StdIn:
                case QueryMethod.StdNotIn:
                case QueryMethod.GreaterThan:
                case QueryMethod.GreaterThanOrEqual:
                case QueryMethod.Equal:
                case QueryMethod.LessThan:
                case QueryMethod.LessThanOrEqual:
                case QueryMethod.NotEqual:
                    paraDic.Add(item.Field + i, item.Value);
                    break;

                default:
                    paraDic.Add(item.Field + i, item.Value);
                    break;
            }
        }
        return paraDic;
    }

    /// <summary>
    /// 将Method转换成sql语法的查询语句
    /// </summary>
    /// <param name="method"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private string ConvertMethodToSql(QueryMethod method, object value)
    {
        switch (method)
        {
            ////字符串类型处理
            case QueryMethod.Contains:
                return "like '%" + value + "%'";

            case QueryMethod.StdIn:
                return "in ('" + string.Join("','", value as string[]) + "')";

            case QueryMethod.NotLike:
                return "not like '%" + value + "%'";
            ////数字类型处理
            case QueryMethod.GreaterThan:
                return "> '" + value + "'";

            case QueryMethod.GreaterThanOrEqual:
                return ">= '" + value + "'";

            case QueryMethod.Equal:
                return "= '" + value + "'";

            case QueryMethod.LessThan:
                return "< '" + value + "'";

            case QueryMethod.LessThanOrEqual:
                return "<= '" + value + "'";

            case QueryMethod.StdNotIn:
                return "not in  ('" + string.Join("','", value as string[]) + "')";

            case QueryMethod.NotEqual:
                return "<> '" + value + "'";

            case QueryMethod.StartsWith:
                return "like '" + value + "%'";

            case QueryMethod.EndsWith:
                return "like '%" + value + "'";

            default:
                return "";
        }
    }
    /// <summary>
    /// 处理空值查询，null值查询
    /// </summary>
    public void BuildEmptySearch()
    {
        if (QueryModel != null && QueryModel.Items != null && QueryModel.Items.Count > 0)
        {
            foreach (var item in QueryModel.Items.ToArray())
            {
                //处理空值查询, 不需要处理已分组的查询条件
                if (item.Value != null && item.Value.ToString() == "" && string.IsNullOrEmpty(item.OrGroup) && item.Method == QueryMethod.Equal)
                {
                    QueryModel.Items.Remove(item);
                    var newGroupId = Guid.NewGuid().ToString("N");
                    QueryModel.Items.Add(new ConditionItem
                    {
                        Field = item.Field,
                        Method = QueryMethod.Equal,
                        Value = null,
                        Prefix = item.Prefix,
                        OrGroup = newGroupId
                    });
                    QueryModel.Items.Add(new ConditionItem
                    {
                        Field = item.Field,
                        Method = QueryMethod.Equal,
                        Value = "",
                        Prefix = item.Prefix,
                        OrGroup = newGroupId
                    });
                }
                //处理不等于，不包含 算上null值 , 不需要处理已分组的查询条件
                if ((item.Method == QueryMethod.NotEqual || item.Method == QueryMethod.NotLike) && string.IsNullOrEmpty(item.OrGroup))
                {
                    QueryModel.Items.Remove(item);
                    item.OrGroup = Guid.NewGuid().ToString("N");
                    QueryModel.Items.Add(item);

                    if (item.Value != null && !IsSimple(item.Value.GetType()))
                    {
                        //不是简单类型，并且不为空值，才加上null判断
                        QueryModel.Items.Add(new ConditionItem
                        {
                            Field = item.Field,
                            Method = QueryMethod.Equal,
                            Value = null,
                            Prefix = item.Prefix,
                            OrGroup = item.OrGroup
                        });
                    }
                }
            }
        }
    }
    /// <summary>
    /// 是否是简单类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool IsSimple(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // Nullable 不是简单类型
            return false;
        }
        return type.IsPrimitive
               || type.IsEnum
               || type.Equals(typeof(string))
               || type.Equals(typeof(decimal));
    }

    /// <summary>
    /// 是否需要做真实的查询请求
    /// <para>
    /// 1,QueryModel里只有一个且为StdIn 时,判断Value集合内个数为0 ,则不需要要查询
    /// </para>
    /// </summary>
    /// <param name="sp"></param>
    /// <returns></returns>
    public static bool IsNeedQuery(SearchParameters sp)
    {
        if (sp.QueryModel.Items.Count != 1)
            return true;
        if (sp.QueryModel.Items[0].Method != QueryMethod.StdIn)
            return true;
        var v = sp.QueryModel.Items[0].Value;
        if (v == null)
            return false;
        if (v is Array va)
            return va.Length != 0;
        if (v is ICollection vc)
            return vc.Count != 0;
        if (v is IEnumerable ve)
            return ve.GetEnumerator().MoveNext();
        else
            throw new ArgumentException("When the Method of ConditionItem is StdIn, Value must be either Array or ICollection");
    }


    /// <summary>
    /// 配合IsNeedQuery方法,尝试创建一个空的返回值
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="r"></param>
    /// <returns></returns>
    public static bool TryGetEmptyResult<R>(out R r)
    {
        var typeR = typeof(R);
        bool a = TryGetEmptyResult(typeR, typeR, out object obj);
        r = (R)obj;
        return a;
    }

    private static bool TryGetEmptyResult(Type OriginalType, Type MatchingType, out object r)
    {
        if (MatchingType.IsValueType)
        {
            r = Activator.CreateInstance(OriginalType);
            return false;
        }

        if (MatchingType.IsGenericType)
        {
            if (MatchingType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {
                var lst = Activator.CreateInstance(OriginalType);
                if (lst != null)
                {
                    r = lst;
                    return true;
                }
            }

            if (MatchingType.GetGenericTypeDefinition().IsAssignableFrom(typeof(PageResult<>)))
            {
                var pageResult = Activator.CreateInstance(OriginalType);
                //Page属性
                var pageProperty = OriginalType.GetProperty("Page");
                if (pageProperty.GetValue(pageResult) == null)
                    pageProperty.SetValue(pageResult, new PageInfo());

                //ListValue属性
                var lstProperty = OriginalType.GetProperty("ListValue");
                if (lstProperty.GetValue(pageResult) == null)
                {
                    var lst = Activator.CreateInstance(lstProperty.PropertyType);
                    lstProperty.SetValue(pageResult, lst); //ListValue属性赋值
                }
                if (lstProperty.GetValue(pageResult) != null)
                {
                    r = pageResult;
                    return true;
                }
            }
            return TryGetEmptyResult(OriginalType, MatchingType.BaseType, out r);
        }
        if (typeof(object) != MatchingType)
        {
            return TryGetEmptyResult(OriginalType, MatchingType.BaseType, out r);
        }
        r = null;
        return false;
    }

}
