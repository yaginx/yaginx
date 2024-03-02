using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted.Commiters
{
    //public class BloggingContextFactory : IDesignTimeDbContextFactory<CenterDbContext>
    //{
    //    public CenterDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<CenterDbContext>();
    //        optionsBuilder.UseNpgsql("Server=192.168.8.80;Port=5432;Database=laoshi;User Id=laoshi;Password=123456;Pooling=true;Application Name=Laoshi.Site;Include Error Detail=true;").UseSnakeCaseNamingConvention();

    //        return new CenterDbContext(optionsBuilder.Options);
    //    }
    //}

    internal class DefaultAutoCommiterStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                Console.WriteLine("AutoCommiter");
                app.UseMiddleware<AutoCommiterMiddleware>();
                next(app);
            };
        }
    }
}
