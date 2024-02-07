using AgileLabs.Caching.Redis;
using StackExchange.Redis;

namespace Yaginx.Infrastructure.ProxyConfigProviders
{
	public class ProxyRuleChangeReceiveService : BackgroundService
	{
		private readonly IRedisStore _redisClient;
		private readonly ProxyRuleChangeNotifyService _proxyRuleChangeNotifyService;

		public ProxyRuleChangeReceiveService(IRedisStore redisClient, ProxyRuleChangeNotifyService proxyRuleChangeNotifyService)
		{
			_redisClient = redisClient;
			_proxyRuleChangeNotifyService = proxyRuleChangeNotifyService;
		}

		/// <summary>
		/// Gateway节点收到动态代理规则发生变更的通知后, 更新本地的路由规则
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var connection = await _redisClient.GetConnectionMultiplexer();
			var subscriber = connection.GetSubscriber();
			subscriber.Subscribe(new RedisChannel(ProxyRuleChangeNotifyService.GATEWAY_PROXY_RULE_EVENT_CHANNEL_NAME, RedisChannel.PatternMode.Literal), (channel, message) =>
			{
				_proxyRuleChangeNotifyService.RedisConfigChanged();
			});
			await Task.CompletedTask;
		}
	}
}
