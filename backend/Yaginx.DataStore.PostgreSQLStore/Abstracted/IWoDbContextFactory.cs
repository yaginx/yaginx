using Microsoft.EntityFrameworkCore;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    public interface IWoDbContextFactory : IAsyncDisposable
    {
        //IDbConnection DbConnection { get; }
        Task<DbContext> GetDefaultDbContextAsync(bool isCreateOnDbContextIsNull = true);
    }
}
