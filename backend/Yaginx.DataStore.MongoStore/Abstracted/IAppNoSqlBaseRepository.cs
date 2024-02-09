using AgileLabs.Storage.Mongo;

namespace Yaginx.DataStore.MongoStore.Abstracted
{
    public interface IAppNoSqlBaseRepository<TEntity> : INoSqlBaseRepository<TEntity>
   where TEntity : MongoEntityBase
    {
    }
}
