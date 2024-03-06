using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Cryptography;
using System.Text;

namespace AgileLabs.EfCore.PostgreSQL.ConnectionStrings
{
    public class DbDataSourceManager : IDbDataSourceManager
    {
        private readonly IConfiguration _configuration;
        private readonly IWorkContextCore _workContext;
        private readonly IMemoryCache _memoryCache;
        private readonly SHA1 _hashAlgorithm;

        public string Id { get; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DbDataSourceManager(
            IConfiguration configuration,
            IWorkContextCore workContext, IMemoryCache memoryCache)
        {
            Id = Guid.NewGuid().ToString("N");
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
            _memoryCache = memoryCache;
            _hashAlgorithm = SHA1.Create();
        }

        public async Task<DbDataSource> GetDbDataSourceAsync()
        {
            string connectonString = _configuration.GetConnectionString("Default");
            return await GetDbDataSourceAsync(connectonString);
        }

        public async Task<DbDataSource> GetDbDataSourceAsync(string connectonString)
        {
            if (string.IsNullOrEmpty(connectonString))
            {
                throw new InvalidOperationException($"Db {nameof(connectonString)} 不能为空");
            }


            var connStringHash = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(connectonString));
            var connStringKey = $"{Convert.ToBase64String(connStringHash)}";
            await Task.CompletedTask;
            return _memoryCache.GetOrCreate(connStringKey, entry =>
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(PopulateRuntimeParameters(connectonString));
                dataSourceBuilder.EnableDynamicJson();
                dataSourceBuilder.UseJsonNet();
                var dataSource = dataSourceBuilder.Build();
                return dataSource;
            });
        }

        private string PopulateRuntimeParameters(string originalConnectionString)
        {
            Dictionary<string, string> dictionary = (from x in originalConnectionString.Split(';')
                                                     where x.Contains("=")
                                                     select x).Select(delegate (string x)
                                                     {
                                                         string[] array = x.Split('=');
                                                         return new Tuple<string, string>(array[0], array[1]);
                                                     }).ToDictionary((key) => key.Item1, (value) => value.Item2);
            dictionary.TryAdd("Application Name", $"{Assembly.GetEntryAssembly().GetName().Name}/{_workContext.Identity?.Id}");
            dictionary.TryAdd("Connection Idle Lifetime", "60");
            return string.Join(";", dictionary.Select((x) => x.Key + "=" + x.Value));
        }
    }
}
