using System.Data.Common;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    public interface IDbDataSourceManager
    {
        string Id { get; }

        /// <summary>
        /// 获取当前子系统的ConectionString
        /// </summary>
        /// <returns></returns>
        Task<DbDataSource> GetDbDataSourceAsync();
    }
}
