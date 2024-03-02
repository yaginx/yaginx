using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class ResourceMonitorInfoEntity
    {
        public string ResourceUuid { get; set; }
        public string SessionKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public List<MonitorInfo> Data { get; set; }
    }
    public class ResourceMonitorInfoEntityTypeConfiguration : IEntityTypeConfiguration<ResourceMonitorInfoEntity>
    {
        public void Configure(EntityTypeBuilder<ResourceMonitorInfoEntity> builder)
        {
            builder.ToTable(nameof(ResourceMonitorInfoEntity).FormatToTableName()).HasKey(x => new { x.ResourceUuid, x.Timestamp });
            builder.Property(p => p.Data).HasColumnType("jsonb");
        }
    }
}
