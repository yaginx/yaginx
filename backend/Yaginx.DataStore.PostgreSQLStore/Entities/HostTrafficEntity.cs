using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AgileLabs.EfCore.PostgreSQL;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class HostTrafficEntity
    {
        public long Id { get; set; }
        public string HostName { get; set; }
        public long Period { get; set; }
        public long RequestCounts { get; set; }
        public long InboundBytes { get; set; }
        public long OutboundBytes { get; set; }
    }
    public class HostTrafficEntityTypeConfiguration : IEntityTypeConfiguration<HostTrafficEntity>
    {
        public void Configure(EntityTypeBuilder<HostTrafficEntity> builder)
        {
            builder.ToTable(nameof(HostTrafficEntity).FormatToTableName()).HasKey(x => x.Id);
        }
    }
}
