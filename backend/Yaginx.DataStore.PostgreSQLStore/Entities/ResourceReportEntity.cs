using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Yaginx.DomainModels.MonitorModels;
using AgileLabs.EfCore.PostgreSQL;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class ResourceReportEntity
    {
        public string ResourceUuid { get; set; }
        public ReportCycleType CycleType { get; set; }
        public DateTimeOffset ReportTime { get; set; }

        /// <summary>
        /// 总处理请求
        /// </summary>
        public long RequestQty { get; set; }
        public Dictionary<string, long> StatusCode { get; set; }
        public Dictionary<string, long> Spider { get; set; }
        public Dictionary<string, long> Browser { get; set; }
        public Dictionary<string, long> Os { get; set; }
        public Dictionary<string, long> Duration { get; set; }

        /// <summary>
        /// 数据统计时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }
    }
    public class ResourceReportEntityTypeConfiguration : IEntityTypeConfiguration<ResourceReportEntity>
    {
        public void Configure(EntityTypeBuilder<ResourceReportEntity> builder)
        {
            builder.ToTable(nameof(ResourceReportEntity).FormatToTableName()).HasKey(x => new { x.ResourceUuid, x.CycleType, x.ReportTime });
            builder.Property(p => p.StatusCode).HasColumnType("jsonb");
            builder.Property(p => p.Spider).HasColumnType("jsonb");
            builder.Property(p => p.Browser).HasColumnType("jsonb");
            builder.Property(p => p.Os).HasColumnType("jsonb");
            builder.Property(p => p.Duration).HasColumnType("jsonb");
        }
    }
}
