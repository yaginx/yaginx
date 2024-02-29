using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class UserEntity
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable(nameof(UserEntity).FormatToTableName()).HasKey(x => x.Id);
        }
    }
    public static class StringExtensions
    {
        private const string EndfixEntity = "Entity";
        public static string FormatToTableName(this string value)
        {
            if (value.EndsWith(EndfixEntity))
            {
                value = value.Substring(0, value.Length - EndfixEntity.Length);
            }
            return Regex.Replace(value, @"([a-z])([A-Z])", "$1_$2").ToLower();
        }
    }
}
