using AutoMapper;
using Yaginx.DomainModels;

namespace Yaginx.Models
{
    public class ReplaceNewImageRequest
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public List<string> Envs { get; set; }
        public List<KeyValuePair<string, List<string>>> Ports { get; set; }
        public List<string> Volumns { get; set; }
        public List<string> Links { get; set; }
        public List<string> Hosts { get; set; }
    }

    public class WebsiteUpsertRequest
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Ĭ�ϵ�����ͷ
        /// </summary>
        public string DefaultHost { get; set; }

        /// <summary>
        /// Ĭ�ϵ�ת����ַ
        /// </summary>
        public string DefaultDestination { get; set; }

        public List<WebsiteHostItem> Hosts { get; set; }
        public List<WebsiteProxyRuleItem> ProxyRules { get; set; }
    }

    public class WebSiteMapping : Profile
    {
        public WebSiteMapping()
        {
            CreateMap<WebsiteUpsertRequest, Website>();
        }
    }
}
