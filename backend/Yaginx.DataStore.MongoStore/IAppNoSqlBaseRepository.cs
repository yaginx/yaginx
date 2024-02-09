using AgileLabs.Storage.Mongo;

namespace Yaginx.DataStore.MongoStore
{
    public interface IAppNoSqlBaseRepository<TEntity> : INoSqlBaseRepository<TEntity>
   where TEntity : MongoEntityBase
    {
    }
}
