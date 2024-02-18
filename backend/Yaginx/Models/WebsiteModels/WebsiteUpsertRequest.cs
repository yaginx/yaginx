using Yaginx.DomainModels;

namespace Yaginx.Models.WebsiteModels
{
    public abstract class WebsiteBaseProperties
    {
        public string Name { get; set; }

        /// <summary>
        /// 默认的主机头
        /// </summary>
        public string DefaultHost { get; set; }

        /// <summary>
        /// 默认的转发地址
        /// </summary>
        public string DefaultDestination { get; set; }

        public string DefaultDestinationHost { get; set; }
        public string WebProxy { get; set; }
        public bool IsWithOriginalHostHeader { get; set; }
        public bool IsAllowUnsafeSslCertificate { get; set; }
        public List<WebsiteHostItem> Hosts { get; set; }
        public List<WebsiteProxyRuleItem> ProxyRules { get; set; }
        public List<KeyValuePair<string, string>> ProxyTransforms { get; set; }
    }

    public class WebsiteUpsertRequest : WebsiteBaseProperties
    {
        public long? Id { get; set; }
    }

    public class WebsiteListItem : WebsiteBaseProperties
    {
        public long Id { get; set; }
        public bool IsHaveSslCert { get; set; }
    }
}
