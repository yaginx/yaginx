using System.Linq.Expressions;
using LiteDB;
using LiteDB.Async;

namespace Yaginx.DataStore.LiteDBStore
{
    public class LiteDbDatabaseRepository
    {
        private enum OperationMode
        {
            Read,
            Write
        }
        private readonly string _connString;
        private ManualResetEvent _writeLocker = new ManualResetEvent(true);
        public LiteDbDatabaseRepository(string connString)
        {
            _connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        //public void StartupInspect(Action<ILiteDatabaseAsync> inspectAction)
        //{
        //    DatabaseAction(inspectAction);
        //}

        public async Task<IEnumerable<T>> SearchAsync<T>(Expression<Func<T, bool>> predicate = null, int skip = 0, int limit = int.MaxValue, Func<IEnumerable<T>, IEnumerable<T>> filters = null)
        {
            if (predicate == null)
                predicate = x => true;

            return await DatabaseActionAsync(async db =>
            {
                var query = await db.GetCollection<T>(typeof(T).Name).FindAsync(predicate, skip, limit);
                query = filters == null ? query : filters(query);
                return query.ToList();
            });
        }

        public Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return DatabaseActionAsync(db => db.GetCollection<T>(typeof(T).Name).FindOneAsync(predicate));
        }

        public Task<T> GetAsync<T>(BsonValue id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return DatabaseActionAsync(db => db.GetCollection<T>(typeof(T).Name).FindByIdAsync(id));
        }

        public Task<bool> UpdateAsync<T>(BsonValue id, T item) => DatabaseActionAsync(db => db.GetCollection<T>(typeof(T).Name).UpdateAsync(id, item));

        public Task<BsonValue> InsertAsync<T>(T item) => DatabaseActionAsync(db => db.GetCollection<T>(typeof(T).Name).InsertAsync(item));
        public Task<int> InsertAsync<T>(List<T> items, int batchSize = 5000) => DatabaseActionAsync(db => db.GetCollection<T>(typeof(T).Name).InsertBulkAsync(items, batchSize));

        public Task DeleteAsync<T>(BsonValue id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return DatabaseActionAsync(db => db.GetCollection<T>(typeof(T).Name).DeleteAsync(id));
        }
        public Task DeleteAsync<T>(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return DatabaseActionAsync(db => db.GetCollection<T>(typeof(T).Name).DeleteManyAsync(predicate));
        }
        public Task<int> TruncateAsync<T>()
        {
            return DatabaseActionAsync(db => db.GetCollection<T>(typeof(T).Name).DeleteAllAsync());
        }

        #region Utils
        private Task<TResult> DatabaseActionAsync<TResult>(Func<ILiteDatabaseAsync, Task<TResult>> action, OperationMode operationMode = OperationMode.Write)
        {
            if (operationMode == OperationMode.Write)
            {
                _writeLocker.WaitOne();
            }

            var connectionString = new ConnectionString(_connString);
            connectionString.Connection = ConnectionType.Shared;
            using var database = new LiteDatabaseAsync(connectionString);
            var result = action(database);
            if (operationMode == OperationMode.Write)
            {
                _writeLocker.Set();
            }
            return result;
        }
        private Task DatabaseActionAsync(Func<ILiteDatabaseAsync, Task> action, OperationMode operationMode = OperationMode.Write) => DatabaseActionAsync(db => action(db), operationMode);
        #endregion
    }
}
