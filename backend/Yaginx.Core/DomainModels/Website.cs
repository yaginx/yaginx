﻿namespace Yaginx.DomainModels
{
    /// <summary>
    /// 站点
    /// </summary>
    public class Website
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
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public interface IWebsiteRepository
    {
        List<Website> Search();
        void Add(Website website);
        void Update(Website website);
        Website Get(long id);
        Website GetByName(string name);
    }

    public class HostTraffic
    {
        public long Id { get; set; }
        public string HostName { get; set; }
        public long RequestCounts { get; set; }
        public long InboundBytes { get; set; }
        public long OutboundBytes { get; set; }
    }

    public interface IHostTrafficRepository
    {
        void Upsert(HostTraffic hostTraffic);
        List<HostTraffic> Search();
        List<HostTraffic> Search(string hostName);
    }
}
