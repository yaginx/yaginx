using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Yaginx.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Hello Yaginx!");
        }

        public IActionResult MemoryCacheTest([FromServices] IMemoryCache memoryCache)
        {
            var result = memoryCache.GetOrCreate("test", cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromDays(365);
                return DateTime.Now.ToString();
            });
            return Content(result);
        }
    }
}
