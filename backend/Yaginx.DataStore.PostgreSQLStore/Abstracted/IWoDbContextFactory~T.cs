using Microsoft.EntityFrameworkCore;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    internal interface IWoDbContextFactory<T> : IWoDbContextFactory, IAsyncDisposable
     where T : DbContext
    {
        Task<T> GetDbContextAsync();
    }
}
