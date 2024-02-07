using AgileLabs.Caching.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Yaginx.Infrastructure.ProxyConfigProviders
{
	public class ProxyRuleRedisStorageService
	{
		const string ProxyRuleHashSetKey = "proxy_rules";
		const string HashSetKey_RuleConfigJson = "rule_config_json";//代理规则Json
		const string HashSetKey_UpdateTime = "last_update_time";//记录上次更新的时间
		const string HashSetKey_UpdateHost = "last_update_host";//记录上次更新的host
		private readonly IRedisClient _redisClient;

		public ProxyRuleRedisStorageService(IRedisClient redisClient)
		{
			_redisClient = redisClient;
		}

		/// <summary>
		/// 从Redis获取所有规则
		/// </summary>
		/// <returns></returns>
		public async Task<IList<YarpRule>> GetRules()
		{
			var cacheValue = await _redisClient.HashGetAsync(ProxyRuleHashSetKey, HashSetKey_RuleConfigJson);
			if (!cacheValue.HasValue)
				return new List<YarpRule>();
			return JsonConvert.DeserializeObject<List<YarpRule>>(cacheValue);
		}

		/// <summary>
		/// 设置所有规则到Redis
		/// </summary>
		/// <param name="rules"></param>
		/// <returns></returns>
		public async Task SetRules(IList<YarpRule> rules)
		{
			if (rules == null || !rules.Any())
				return;

			var cacheValue = JsonConvert.SerializeObject(rules);
			await _redisClient.HashSetAsync(ProxyRuleHashSetKey, HashSetKey_RuleConfigJson, cacheValue);

			var dic = new Dictionary<string, string>
			{
				{ HashSetKey_RuleConfigJson, cacheValue },
				{ HashSetKey_UpdateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
				{ HashSetKey_UpdateHost, Environment.MachineName }
			};

			await _redisClient.HashSetAsync(ProxyRuleHashSetKey, dic.Select(x => new HashEntry(x.Key, x.Value)).ToArray());
		}
	}
}
