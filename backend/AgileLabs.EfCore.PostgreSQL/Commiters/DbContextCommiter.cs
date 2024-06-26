﻿using AgileLabs.EfCore.PostgreSQL.ContextFactories;

namespace AgileLabs.EfCore.PostgreSQL.Commiters
{
    /// <summary>
    /// DbContext提交器
    /// </summary>
    public class DbContextCommiter : IDbContextCommiter
    {
        private readonly IWorkContextCore _workContextCore;
        public bool IsDbContextCreated { get; set; }

        public DbContextCommiter(IWorkContextCore workContextCore)
        {
            _workContextCore = workContextCore;
        }


        public async Task CommitAsync()
        {
            if (IsDbContextCreated)
            {
                var woDbContextFactory = _workContextCore.Resolve<IAgileLabDbContextFactory>();
                var dbContext = await woDbContextFactory.GetDefaultDbContextAsync(false);
                if (dbContext != null)
                    await dbContext.SaveChangesAsync();
            }
        }
    }
}
