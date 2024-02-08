namespace Yaginx.DomainModels
{
    /// <summary>
    /// 站点
    /// </summary>
    public class Website
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public List<WebsiteHostItem> Hosts { get; set; }
        public List<WebsiteProxyRuleItem> ProxyRules { get; set; }
    }

    public interface IWebsiteRepository
    {
        List<Website> Search();
        void Add(Website website);
        void Update(Website website);
        Website Get(long id);
        Website GetByName(string name);
    }
}
