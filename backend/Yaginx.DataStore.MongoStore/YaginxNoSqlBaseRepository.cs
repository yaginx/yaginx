using AgileLabs.Storage.Mongo;
using Microsoft.Extensions.Logging;

namespace Yaginx.DataStore.MongoStore
{
    public class YaginxNoSqlBaseRepository<TEntity> : NoSqlBaseRepository<TEntity, MongodbSetting>, IAppNoSqlBaseRepository<TEntity>
  where TEntity : MongoEntityBase
    {

        public YaginxNoSqlBaseRepository(MongodbContext<MongodbSetting> mongoDatabase, ILogger<YaginxNoSqlBaseRepository<TEntity>> logger)
            : base(mongoDatabase, logger)
        {
        }
    }
}
