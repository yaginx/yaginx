using Yaginx.DomainModels;

namespace Yaginx.Models
{
    public class WebsiteUpsertRequest
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 默认的主机头
        /// </summary>
        public string DefaultHost { get; set; }

        /// <summary>
        /// 默认的转发地址
        /// </summary>
        public string DefaultDestination { get; set; }

        public List<WebsiteHostItem> Hosts { get; set; }
        public List<WebsiteProxyRuleItem> ProxyRules { get; set; }
    }
}
