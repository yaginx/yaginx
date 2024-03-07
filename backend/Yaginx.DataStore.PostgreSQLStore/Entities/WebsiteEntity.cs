using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Yaginx.DomainModels;
using System.Reflection.Emit;
using AgileLabs.EfCore.PostgreSQL;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class WebsiteEntity
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public virtual WebsiteSpecifications Specifications { get; set; }

        public string[] Hosts { get; set; }
        public List<WebsiteProxyRuleItem> ProxyRules { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset UpdateTime { get; set; }

        public Dictionary<string, string> ProxyTransforms { get; set; }
        public SimpleResponseItem[] SimpleResponses { get; set; }
    }

    public class WebsiteEntityTypeConfiguration : IEntityTypeConfiguration<WebsiteEntity>
    {
        public void Configure(EntityTypeBuilder<WebsiteEntity> builder)
        {
            builder.ToTable(nameof(WebsiteEntity).FormatToTableName()).HasKey(x => x.Id);

            //builder.OwnsMany(product => product.ProxyRules, builder => { builder.ToJson(); })
            //.OwnsMany(product => product.SimpleResponses, builder => { builder.ToJson(); });
            builder.Property(p => p.Specifications).HasColumnType("jsonb");
            builder.Property(p => p.ProxyRules).HasColumnType("jsonb");
            builder.Property(p => p.SimpleResponses).HasColumnType("jsonb");
            builder.Property(p => p.Hosts).HasColumnType("jsonb");
            builder.Property(p => p.ProxyTransforms).HasColumnType("jsonb");
        }
    }
}
