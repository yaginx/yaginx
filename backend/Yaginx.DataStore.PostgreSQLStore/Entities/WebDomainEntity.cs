using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class WebDomainEntity
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public bool IsUseFreeCert { get; set; }
        public bool IsVerified { get; set; }
        public string FreeCertMessage { get; set; }
    }

    public class WebDomainEntityTypeConfiguration : IEntityTypeConfiguration<WebDomainEntity>
    {
        public void Configure(EntityTypeBuilder<WebDomainEntity> builder)
        {
            builder.ToTable(nameof(WebDomainEntity).FormatToTableName()).HasKey(x => x.Id);
        }
    }
}
