using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Yaginx.Infrastructure
{
    public class AntdTableSearchParametersBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var bodyStream = bindingContext.HttpContext.Request.Body;
            using var streamReader = new StreamReader(bodyStream);
            var body = await streamReader.ReadToEndAsync();

            var model = new AntdTableSearchParameters() { PageSize = 50, Current = 1 };
            var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(body);
            if (!data.Any())
            {
                bindingContext.Result = ModelBindingResult.Success(model);
            }

            foreach (var item in data)
            {
                if (item.Value == null)
                    continue;
                switch (item.Key)
                {
                    case "current":
                        model.Current = Convert.ToInt32(item.Value);
                        break;
                    case "pageSize":
                        model.PageSize = Convert.ToInt32(item.Value);
                        break;
                    case "pageSizeOptions":
                    case "pageIndex":
                    case "total":
                        break;
                    default:
                        var camalCaseKey = item.Key.ToPascalCase();
                        var valueType = item.Value.GetType();
                        if (valueType.IsPrimitive)
                        {
                            model.Filters.TryAdd(camalCaseKey, item.Value);
                        }
                        else
                        {
                            switch (item.Value.GetType().Name)
                            {
                                case "JArray":
                                    var jsonArray = (JArray)item.Value;
                                    if (jsonArray.IsNullOrEmpty())
                                        break;

                                    var values = jsonArray.Select(x => ((JValue)x).Value).ToList();
                                    model.Filters.TryAdd(camalCaseKey, values.Count > 1 ? values.Select(x =>
                                    {
                                        var xType = x.GetType().Name;

                                        return x;
                                    }).ToList() : values.FirstOrDefault());
                                    break;
                                default:
                                    model.Filters.TryAdd(camalCaseKey, item.Value);
                                    break;
                            }
                        }
                        break;
                }
            }
            bindingContext.Result = ModelBindingResult.Success(model.ConvertToSearchParameters());
        }
    }
}
