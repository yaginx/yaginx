using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaginx.DomainModels
{
    /// <summary>
    /// 网站域名
    /// </summary>
    public class WebDomain
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public bool IsVerified { get; set; }
    }

    public interface IWebDomainRepository
    {
        List<WebDomain> Search();
        void Add(WebDomain webDomain);
        void Update(WebDomain webDomain);
        WebDomain Get(long id);
        WebDomain GetByName(string name);
    }
}
