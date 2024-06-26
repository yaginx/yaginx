using AgileLabs.Caching.Redis;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;

namespace Yaginx.Infrastructure
{
    public class ProxyRuleChangeNotifyService
    {
        public const string GATEWAY_PROXY_RULE_EVENT_CHANNEL_NAME = nameof(GATEWAY_PROXY_RULE_EVENT_CHANNEL_NAME);

        private readonly IRedisStore _redisClient;
        private readonly ILogger<ProxyRuleChangeNotifyService> _logger;

        public ProxyRuleChangeNotifyService(IRedisStore redisClient, ILogger<ProxyRuleChangeNotifyService> logger)
        {
            _redisClient = redisClient;
            _logger = logger;
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
        private CancellationTokenSource _tokenSourceProxyRule;
        private CancellationTokenSource _tokenSourceSimpleProcessor;

        /// <summary>
        /// 获取CancellationChangeToken实例方法
        /// </summary>
        public CancellationChangeToken CreateProxyChanageToken()
        {
            _tokenSourceProxyRule = new CancellationTokenSource();
            return new CancellationChangeToken(_tokenSourceProxyRule.Token);
        }

        public CancellationChangeToken CreateSimpleProcessorChanageToken()
        {
            _tokenSourceSimpleProcessor = new CancellationTokenSource();
            return new CancellationChangeToken(_tokenSourceSimpleProcessor.Token);
        }

        /// <summary>
        /// 取消CancellationTokenSource
        /// </summary>
        public void ProxyRuleConfigChanged()
        {
            _logger.LogWarning($"Proxy Rules Config Changed!");
            _tokenSourceProxyRule.Cancel();
            _tokenSourceSimpleProcessor.Cancel();
            _logger.LogWarning($"Proxy Rules Config Changed Notify Success!");
        }
        #endregion
    }
}
