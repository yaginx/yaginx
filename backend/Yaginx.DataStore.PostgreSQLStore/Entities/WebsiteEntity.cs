using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Yaginx.DomainModels;
using System.Reflection.Emit;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class WebsiteEntity
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 默认的主机头(请求主机头)
        /// </summary>
        public string DefaultHost { get; set; }

        /// <summary>
        /// 默认的转发地址
        /// </summary>
        public string DefaultDestination { get; set; }

        /// <summary>
        /// 转发的Host
        /// </summary>
        public string DefaultDestinationHost { get; set; }

        /// <summary>
        /// 使用代理地址
        /// </summary>
        public string WebProxy { get; set; }

        /// <summary>
        /// 是否忽略SSL证书检查
        /// </summary>
        public bool IsAllowUnsafeSslCertificate { get; set; }

        /// <summary>
        /// 是否携带源主机头
        /// </summary>
        public bool IsWithOriginalHostHeader { get; set; }
        public bool IsAutoRedirectHttp2Https { get; set; }

        public string[] Hosts { get; set; }
        public List<WebsiteProxyRuleItem> ProxyRules { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public Dictionary<string, string> ProxyTransforms { get; set; }
        public SimpleResponseItem[] SimpleResponses { get; set; }
    }

    public class WebsiteEntityTypeConfiguration : IEntityTypeConfiguration<WebsiteEntity>
    {
        public void Configure(EntityTypeBuilder<WebsiteEntity> builder)
        {
            builder.ToTable(nameof(WebsiteEntity).FormatToTableName()).HasKey(x => x.Id);

            builder.OwnsMany(product => product.ProxyRules, builder => { builder.ToJson(); })
            .OwnsMany(product => product.SimpleResponses, builder => { builder.ToJson(); });

            builder.Property(p => p.Hosts).HasColumnType("jsonb").IsRequired();
            builder.Property(p => p.ProxyTransforms).HasColumnType("jsonb").IsRequired();
        }
    }
}
