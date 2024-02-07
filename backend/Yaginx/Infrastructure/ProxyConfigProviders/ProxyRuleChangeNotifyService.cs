using AgileLabs.Caching.Redis;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;

namespace Yaginx.Infrastructure.ProxyConfigProviders
{
	public class ProxyRuleChangeNotifyService
	{
		public const string GATEWAY_PROXY_RULE_EVENT_CHANNEL_NAME = nameof(GATEWAY_PROXY_RULE_EVENT_CHANNEL_NAME);

		private readonly IRedisStore _redisClient;

		public ProxyRuleChangeNotifyService(IRedisStore redisClient)
		{
			_redisClient = redisClient;
		}

		/// <summary>
		/// 通知所有Gateway节点,动态代理规则发生了变更
		/// </summary>
		/// <returns></returns>
		public async Task NotifyAllGatewayNodeTheRuleWasChangedAsync()
		{
			var connection = await _redisClient.GetConnectionMultiplexer();
			var subscriber = connection.GetSubscriber();
			await subscriber.PublishAsync(new RedisChannel(GATEWAY_PROXY_RULE_EVENT_CHANNEL_NAME, RedisChannel.PatternMode.Literal), "1");
		}

		#region Dynamic Update
		private CancellationTokenSource _tokenSource;

		/// <summary>
		/// 获取CancellationChangeToken实例方法
		/// </summary>
		public CancellationChangeToken CreateChanageToken()
		{
			_tokenSource = new CancellationTokenSource();
			return new CancellationChangeToken(_tokenSource.Token);
		}

		/// <summary>
		/// 取消CancellationTokenSource
		/// </summary>
		public void RedisConfigChanged() => _tokenSource.Cancel();
		#endregion
	}
}