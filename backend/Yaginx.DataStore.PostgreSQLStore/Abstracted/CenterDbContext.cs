using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
/*
 列出迁移列表
dotnet ef migrations list

增加一次迁移
dotnet ef migrations add Init

列出脚本
dotnet ef migrations script

删除最近一次迁移
dotnet ef migrations remove

直接更新db
dotnet ef database update

create role yaginx with login encrypted password '123456' connection limit -1;
create database yaginx with owner yaginx encoding='UTF8';
alter database laoshi set timezone to 'Asia/Shanghai';
\c yaginx
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "ltree";
CREATE EXTENSION IF NOT EXISTS "pg_stat_statements"; 
 */
namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    public class CenterDbContext : WoDbContext
    {
        public CenterDbContext(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            //builder.HasPostgresEnum<VerifyChannel>();
            //builder.HasPostgresEnum<VerifyType>();
        }
    }
    public class UserCode
    {
        public long CodeId { get; set; }
        public string CodeType { get; set; }
    }

    public class Account
    {
        public long AcountId { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// Hash加密之后的密码
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the password salt
        /// </summary>
        public string PasswordSalt { get; set; }
    }

    public class AccountRelation
    {
        public long AccountId { get; set; }
        public OrgRole OperandRole { get; set; }
        public long OperandId { get; set; }
    }

    public enum OrgRole
    {
        Teacher = 1,
        Agent = 2
    }

    public class HostCode
    {
        public long HostId { get; set; }
        public string Code { get; set; }

    }

    public class Agent
    {

    }

    public class Teacher
    {

    }

    public class WebPage
    {
        public long PageId { get; set; }
        public string Content { get; set; }
    }
    public class WebPageEntityTypeConfiguration : IEntityTypeConfiguration<WebPage>
    {
        public void Configure(EntityTypeBuilder<WebPage> builder)
        {
            builder.ToTable(nameof(WebPage).ToLowerUnderscoreCase()).HasKey(x => x.PageId);
        }
    }
    public static class StringExtensions
    {
        public static string ToLowerUnderscoreCase(this string value)
        {
            return Regex.Replace(value, @"([a-z])([A-Z])", "$1_$2").ToLower();
        }
    }
}
