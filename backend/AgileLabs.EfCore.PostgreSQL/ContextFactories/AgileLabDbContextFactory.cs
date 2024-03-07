using AgileLabs.EfCore.PostgreSQL.ConnectionStrings;
using AgileLabs.Storage.PostgreSql;

namespace AgileLabs.EfCore.PostgreSQL.ContextFactories
{
    public class AgileLabDbContextFactory<T> : IWoDbContextFactory<T>
    where T : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDbContextCommiter _dbContextCommiter;
        private readonly IConnectionSafeHelper _connectionSafeHelper;
        private readonly IDbDataSourceManager _dbDataSourceManager;
        private DbContext _context;
        private static object _lock = new object();

        public string Id { get; }

        public AgileLabDbContextFactory(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            IDbContextCommiter dbContextCommiter,
            IConnectionSafeHelper connectionSafeHelper,
            IDbDataSourceManager dbDataSourceManager)
        {
            Id = Guid.NewGuid().ToString();
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _dbContextCommiter = dbContextCommiter ?? throw new ArgumentNullException(nameof(dbContextCommiter));
            _connectionSafeHelper = connectionSafeHelper;
            _dbDataSourceManager = dbDataSourceManager;
        }

        public async Task<DbContext> GetDefaultDbContextAsync(bool isCreateOnDbContextIsNull = true)
        {
            if (_context == null && isCreateOnDbContextIsNull)
            {
                try
                {
                    _context = (DbContext)Activator.CreateInstance(typeof(T), _loggerFactory);

                    if (_context == null)
                    {
                        throw new Exception($"{nameof(GetDefaultDbContextAsync)}中创建的{nameof(_context)}对象未null");
                    }

                    if (_context is AgileLabDbContext bizDbContext)
                    {
                        var dbDataSource = await _dbDataSourceManager.GetDbDataSourceAsync();
                        if (dbDataSource == null)
                        {
                            throw new Exception($"{GetType().Name}创建{typeof(T).FullName}失败, 获取到的{nameof(dbDataSource)}为空");
                        }

                        bizDbContext.DbDataSource = dbDataSource;
                        //bizDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                        _dbContextCommiter.IsDbContextCreated = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(GetDefaultDbContextAsync)}中创建{typeof(T).FullName}失败,错误:{ex.FullMessage()}", ex);
                }
            }
            return _context;
        }

        public async Task<T> GetDbContextAsync()
        {
            var realDbContext = await GetDefaultDbContextAsync();
            var targetDbContext = realDbContext as T;
            if (targetDbContext == null)
            {
                throw new Exception($"{realDbContext.GetType().FullName}不是{typeof(T).FullName}, 无法进行类型互转");
            }
            return targetDbContext;
        }

        #region Dispose
        private bool _isDisposed;

        /// <summary>
        ///     手动调用的释放资源方法
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">设置为True，需要重写DisposeCore函数</param>
        private async ValueTask Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                await DisposeCore();
            }
            _isDisposed = true;
        }
        /// <summary>
        ///     虚方法，在释放资源时指定的函数操作
        /// </summary>
        protected virtual async ValueTask DisposeCore()
        {
            if (_context != null)
                await _context.DisposeAsync();
            //if (DbConnection != null)
            //    await _connectionSafeHelper.CloseConnectionAsync(DbConnection);
        }
        #endregion
    }
}
