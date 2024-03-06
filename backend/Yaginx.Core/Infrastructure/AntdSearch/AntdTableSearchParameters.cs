using Microsoft.AspNetCore.Mvc;

namespace Yaginx.Infrastructure
{
    [ModelBinder(BinderType = typeof(AntdTableSearchParametersBinder))]
    public class AntdTableSearchParameters
    {
        public int Current { get; set; }
        public int PageSize { get; set; }
        public Dictionary<string, object> Filters { get; init; }

        public AntdTableSearchParameters()
        {
            Filters = new Dictionary<string, object>();
        }
    }
}
