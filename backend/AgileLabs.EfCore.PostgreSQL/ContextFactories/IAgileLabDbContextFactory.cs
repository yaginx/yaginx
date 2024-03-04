using Microsoft.EntityFrameworkCore;

namespace AgileLabs.EfCore.PostgreSQL.ContextFactories
{
    public interface IAgileLabDbContextFactory : IAsyncDisposable
    {
        //IDbConnection DbConnection { get; }
        Task<DbContext> GetDefaultDbContextAsync(bool isCreateOnDbContextIsNull = true);
    }
}
