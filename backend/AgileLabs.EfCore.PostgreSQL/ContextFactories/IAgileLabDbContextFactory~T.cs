using Microsoft.EntityFrameworkCore;

namespace AgileLabs.EfCore.PostgreSQL.ContextFactories
{
    internal interface IWoDbContextFactory<T> : IAgileLabDbContextFactory, IAsyncDisposable
     where T : DbContext
    {
        Task<T> GetDbContextAsync();
    }
}
