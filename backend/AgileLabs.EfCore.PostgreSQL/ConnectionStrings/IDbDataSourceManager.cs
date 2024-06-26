﻿namespace AgileLabs.EfCore.PostgreSQL.ConnectionStrings
{
    public interface IDbDataSourceManager
    {
        string Id { get; }

        /// <summary>
        /// 获取当前子系统的ConectionString
        /// </summary>
        /// <returns></returns>
        Task<DbDataSource> GetDbDataSourceAsync();
        Task<DbDataSource> GetDbDataSourceAsync(string connectionString);
    }
}
