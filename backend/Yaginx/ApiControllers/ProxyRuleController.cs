using AgileLabs;
using Microsoft.AspNetCore.Mvc;
using Yaginx.Infrastructure;
using Yaginx.Infrastructure.ProxyConfigProviders;

namespace Yaginx.ApiControllers;
[ApiController, Route("api/proxy_rule")]
public class ProxyRuleController : YaginxControllerBase
{
    #region Proxy Rules
    [HttpGet, Route("proxy_rule/getall")]
    public async Task<IList<YarpRule>> ProxyRuleGetAll([FromServices] ProxyRuleRedisStorageService yarpRuleRedisStorageService)
    {
        var redisRules = await yarpRuleRedisStorageService.GetRules();
        return redisRules;
    }
    [HttpPost, Route("proxy_rule/append")]
    public async Task ProxyRuleAppend([FromBody] List<YarpRule> appendRules, [FromServices] ProxyRuleRedisStorageService yarpRuleRedisStorageService, [FromServices] ProxyRuleChangeNotifyService quantumProxyRuleService)
    {
        if (appendRules.IsNullOrEmpty())
        {
            return;
        }

        var redisRules = await yarpRuleRedisStorageService.GetRules();
        bool isChanged = false;
        foreach (var newRule in appendRules)
        {
            var existRules = redisRules.Where(x => x.RequestPattern.Equals(newRule.RequestPattern, System.StringComparison.OrdinalIgnoreCase)).ToList();
            if (existRules.IsNullOrEmpty())
            {
                // 直接添加新的
                redisRules.Add(newRule);
                isChanged = true;
            }
            else if (existRules.Count() > 1)
            {
                throw new System.Exception($"规则:{newRule.RequestPattern}存在多个同规则配置, 请删除之后重新配置");
            }
            else
            {
                var existRule = existRules.FirstOrDefault();

                // 下面是只有1个的处理逻辑
                if (newRule.Equals(existRule))
                    continue;

                // 如果不同, 删除原来的, 添加新的
                redisRules.Remove(existRule);
                redisRules.Add(newRule);
                isChanged = true;
            }
        }

        if (isChanged)
        {
            await ProxyRulesUpdateRedisValue(yarpRuleRedisStorageService, quantumProxyRuleService, redisRules);
        }
    }

    [HttpPost, Route("proxy_rule/remove")]
    public async Task ProxyRuleRemove([FromBody] List<YarpRule> removeRules, [FromServices] ProxyRuleRedisStorageService yarpRuleRedisStorageService, [FromServices] ProxyRuleChangeNotifyService quantumProxyRuleService)
    {
        if (removeRules.IsNullOrEmpty())
        {
            return;
        }

        var redisRules = await yarpRuleRedisStorageService.GetRules();
        bool isChanged = false;
        foreach (var removeRule in removeRules)
        {
            var existRules = redisRules.Where(x => x.RequestPattern.Equals(removeRule.RequestPattern, System.StringComparison.OrdinalIgnoreCase)).ToList();
            if (existRules.IsNullOrEmpty())
            {
                continue;
            }

            foreach (var existRule in existRules)
            {
                redisRules.Remove(existRule);
            }
            isChanged = true;
        }

        if (isChanged)
        {
            await ProxyRulesUpdateRedisValue(yarpRuleRedisStorageService, quantumProxyRuleService, redisRules);
        }
    }

    /// <summary>
    /// 更新Redis中的值
    /// </summary>
    /// <param name="yarpRuleRedisStorageService"></param>
    /// <param name="quantumProxyRuleService"></param>
    /// <param name="redisRules"></param>
    /// <returns></returns>
    [NonAction]
    private async Task ProxyRulesUpdateRedisValue(ProxyRuleRedisStorageService yarpRuleRedisStorageService, ProxyRuleChangeNotifyService quantumProxyRuleService, IList<YarpRule> redisRules)
    {
        await yarpRuleRedisStorageService.SetRules(redisRules);
        await quantumProxyRuleService.NotifyAllGatewayNodeTheRuleWasChangedAsync();
    }
    #endregion
}
