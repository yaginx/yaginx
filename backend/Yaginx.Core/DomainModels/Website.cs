namespace Yaginx.DomainModels
{
    /// <summary>
    /// 站点
    /// </summary>
    public class Website
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public List<WebsiteDomain> Hosts { get; set; }
        public List<WebsiteProxyRule> ProxyRules { get; set; }
    }

    public class WebsiteDomain
    {
        public string Domain { get; set; }
        public string Certificate { get; set; }
    }
}
