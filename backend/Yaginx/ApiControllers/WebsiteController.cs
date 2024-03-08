using AgileLabs.EfCore.PostgreSQL;
using AgileLabs.FileUpload;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using Yaginx.DomainModels;
using Yaginx.Infrastructure;
using Yaginx.Models.WebsiteModels;

namespace Yaginx.ApiControllers;
[ApiController, Route("yaginx/api/website")]
public class WebsiteController : YaginxControllerBase
{
    private readonly IWebsiteRepository _websiteRepository;
    private readonly ProxyRuleChangeNotifyService _proxyRuleChangeNotifyService;

    public WebsiteController(IWebsiteRepository websiteRepository, ProxyRuleChangeNotifyService proxyRuleChangeNotifyService)
    {
        _websiteRepository = websiteRepository;
        _proxyRuleChangeNotifyService = proxyRuleChangeNotifyService;
    }

    [HttpPost, Route("upsert")]
    public async Task<WebsiteUpsertRequest> Add([FromBody] WebsiteUpsertRequest request)
    {
        WebsiteDomainModel returnValue;
        if (!request.Id.HasValue)
        {
            var site = _mapper.Map<WebsiteDomainModel>(request);
            site.Id = IdGenerator.NextId();
            site.CreateTime = DateTime.Now;
            site.UpdateTime = DateTime.Now;
            await _websiteRepository.AddAsync(site);
            returnValue = site;
        }
        else
        {
            var oldSite = await _websiteRepository.GetAsync(request.Id.Value);
            if (oldSite == null)
            {
                throw new Exception($"Site #{request.Id} not exist");
            }

            _mapper.Map(request, oldSite);

            oldSite.UpdateTime = DateTime.Now;
            await _websiteRepository.UpdateAsync(oldSite);
            returnValue = oldSite;
        }
        _proxyRuleChangeNotifyService.ProxyRuleConfigChanged();
        return _mapper.Map<WebsiteUpsertRequest>(returnValue);
    }

    [HttpDelete, Route("delete")]
    public async Task Delete(long id)
    {
        await _websiteRepository.DeleteAsync(id);
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<WebsiteListItem>> Search()
    {
        var result = await _websiteRepository.SearchAsync();
        return _mapper.Map<List<WebsiteListItem>>(result);
    }

    [HttpGet, Route("get")]
    public async Task<WebsiteListItem> Get(long id)
    {
        var entity = await _websiteRepository.GetAsync(id);
        return _mapper.Map<WebsiteListItem>(entity);
    }

    [HttpGet, Route("get_by_name")]
    public async Task<WebsiteListItem> Get(string name)
    {
        var entity = await _websiteRepository.GetByNameAsync(name);
        return _mapper.Map<WebsiteListItem>(entity);
    }

    [HttpGet, Route("config/backup")]
    public async Task<IActionResult> BackupConfig()
    {
        var result = await _websiteRepository.SearchAsync();
        HttpContext.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=\"website_config_backup.json\"");
        return Content(JsonConvert.SerializeObject(result.ToList()), "application/octet-stream", contentEncoding: Encoding.UTF8);
    }
    [HttpPost]
    [DisableFormValueModelBinding]
    [Route("config/restore")]
    public async Task RestoreConfig()
    {
        var formOptions = HttpContext.RequestServices.GetRequiredService<IOptions<FormOptions>>()?.Value;
        var allowExtensions = new[] { ".json" };
        Func<string, long> getSizeLimitFunc = (ext) =>
        {
            var result = 2L * 1024 * 1024 * 1024;
            switch (ext)
            {
                case ".json":
                    result = 4L * 1024 * 1024 * 1024;
                    break;
                default:
                    break;
            }
            return result;
        };
        var files = await this.ReadUploadFiles(formOptions, allowExtensions, getSizeLimitFunc);
        if (files.Count != 1)
        {
            throw new Exception("只允许上传一个文件");
        }       

        try
        {
            var fileContent = files.First().FileContent;

            string configJson = Encoding.UTF8.GetString(fileContent);

            var websites = JsonConvert.DeserializeObject<List<WebsiteDomainModel>>(configJson);
            foreach (var website in websites)
            {
                await _websiteRepository.UpdateAsync(website);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("恢复失败", ex);
        }
    }
}
