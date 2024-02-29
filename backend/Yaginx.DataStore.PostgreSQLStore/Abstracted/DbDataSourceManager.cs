using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data.Common;
using System.Reflection;
using Yaginx.WorkContexts;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    public class DbDataSourceManager : IDbDataSourceManager
    {
        private readonly IConfiguration _configuration;
        private readonly IWorkContext _workContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<DbDataSourceManager> _logger;

        public string Id { get; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DbDataSourceManager(
            IConfiguration configuration,
            IWorkContext workContext,
            IMemoryCache memoryCache,
            ILogger<DbDataSourceManager> logger)
        {
            Id = Guid.NewGuid().ToString("N");
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<DbDataSource> GetDbDataSourceAsync()
        {
            string connectonString = _configuration.GetConnectionString("Default");

            if (string.IsNullOrEmpty(connectonString))
            {
                throw new InvalidOperationException($"Db {nameof(connectonString)} 不能为空");
            }

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(PopulateRuntimeParameters(connectonString));
            dataSourceBuilder.EnableDynamicJson();
            dataSourceBuilder.UseJsonNet();
            var dataSource = dataSourceBuilder.Build();
            await Task.CompletedTask;
            return dataSource;
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
            dictionary.TryAdd("Application Name", $"{Assembly.GetEntryAssembly().GetName().Name}/{_workContext.Identity.Id}");
            dictionary.TryAdd("Connection Idle Lifetime", "60");
            return string.Join(";", dictionary.Select((x) => x.Key + "=" + x.Value));
        }
    }
}
