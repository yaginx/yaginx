namespace AgileLabs.EfCore.PostgreSQL.DynamicSearch.Model;


[Serializable]
public class QueryModel
{
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public QueryModel()
    {
        Items = new List<ConditionItem>();
    }

    /// <summary>
    ///     查询条件
    /// </summary>
    public List<ConditionItem> Items { get; set; }
}