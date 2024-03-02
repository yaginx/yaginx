using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class AccountEntity
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<AccountEntity>
    {
        public void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.ToTable(nameof(AccountEntity).FormatToTableName()).HasKey(x => x.Id);
        }
    }
}
