namespace AgileLabs.EfCore.PostgreSQL.DynamicSearch.Model;


[Serializable]
public class QueryModel
{
    /// <summary>
    /// Ĭ�Ϲ��캯��
    /// </summary>
    public QueryModel()
    {
        Items = new List<ConditionItem>();
    }

    /// <summary>
    ///     ��ѯ����
    /// </summary>
    public List<ConditionItem> Items { get; set; }
}