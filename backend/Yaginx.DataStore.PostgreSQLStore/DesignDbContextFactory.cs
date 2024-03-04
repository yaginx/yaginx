using AgileLabs.EfCore.PostgreSQL.ContextFactories;
namespace Yaginx.DataStore.PostgreSQLStore
{
    public class DesignDbContextFactory : DesignContextFactoryAbstract<CenterDbContext>
    {
        public DesignDbContextFactory()
            : base("Server=192.168.8.80;Port=5432;Database=yaginx;User Id=yaginx;Password=123456;Pooling=true;Application Name=Yaginx Apps;Include Error Detail=true;")
        {
        }
    }
}
