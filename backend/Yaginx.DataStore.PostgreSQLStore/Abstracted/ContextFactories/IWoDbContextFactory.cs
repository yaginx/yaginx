using Microsoft.EntityFrameworkCore;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted.ContextFactories
{
    public interface IWoDbContextFactory : IAsyncDisposable
    {
        //IDbConnection DbConnection { get; }
        Task<DbContext> GetDefaultDbContextAsync(bool isCreateOnDbContextIsNull = true);
    }
}
