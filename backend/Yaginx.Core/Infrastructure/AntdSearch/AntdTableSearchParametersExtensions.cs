using AgileLabs.EfCore.PostgreSQL.DynamicSearch;
using AgileLabs.EfCore.PostgreSQL.DynamicSearch.Model;

namespace Yaginx.Infrastructure
{
    public static class AntdTableSearchParametersExtensions
    {
        public static SearchParameters ConvertToSearchParameters(this AntdTableSearchParameters obj)
        {
            var sp = new SearchParameters
            {
                PageInfo = new PageInfo { CurrentPage = obj.Current, PageSize = obj.PageSize },
                QueryModel = new QueryModel()
            };

            if (obj.Current <= 0)
            {
                sp.PageInfo.CurrentPage = 1;
            }
            if (obj.PageSize <= 0)
            {
                sp.PageInfo.PageSize = 20;
            }

            foreach (var item in obj.Filters)
            {
                switch (item.Value.GetType().Name)
                {
                    case nameof(Int32):
                    case nameof(Int64):
                        sp.QueryModel.Items.Add(new ConditionItem { Field = item.Key, Value = item.Value, Method = QueryMethod.Equal });
                        break;
                    case nameof(String):
                        sp.QueryModel.Items.Add(new ConditionItem { Field = item.Key, Value = item.Value, Method = QueryMethod.AutoDetect });
                        break;
                    case "List`1":
                        sp.QueryModel.Items.Add(new ConditionItem { Field = item.Key, Value = item.Value, Method = QueryMethod.StdIn });
                        break;
                    default:
                        sp.QueryModel.Items.Add(new ConditionItem { Field = item.Key, Value = item.Value, Method = QueryMethod.Equal });
                        break;
                }
            }
            return sp;
        }
    }
}
