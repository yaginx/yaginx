using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Yaginx.DataStore.LiteDBStore;
using Yaginx.DataStore.LiteDBStore.Repositories;
using Yaginx.DomainModels;

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

        [HttpGet]
        public async Task<IActionResult> Upgrade([FromServices] LiteDbDatabaseRepository liteDbDatabaseRepository,
            [FromServices] IMapper mapper,
            [FromServices] IWebsiteRepository websiteRepository,
            [FromServices] IWebDomainRepository webDomainRepository)
        {
            try
            {
                var liteDbWebsites = await new WebsiteRepository(liteDbDatabaseRepository, mapper).SearchAsync();
                foreach (var item in liteDbWebsites)
                {
                    await websiteRepository.AddAsync(item);
                }

                var webDomainList = await new WebDomainRepository(liteDbDatabaseRepository).SearchAsync();
                foreach (var item in webDomainList)
                {
                    await webDomainRepository.AddAsync(item);
                }
                return Content("upgrade success");
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
