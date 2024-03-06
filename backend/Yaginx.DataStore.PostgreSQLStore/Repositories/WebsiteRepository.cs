using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class WebsiteRepository : YaginxCrudBaseRepository<WebsiteDomainModel, WebsiteEntity>, IWebsiteRepository
    {
        public WebsiteRepository(IAgileLabDbContextFactory factory, IMapper mapper, ILogger<WebsiteRepository> logger) : base(factory, mapper, logger)
        {
        }

        public async Task<WebsiteDomainModel> GetByNameAsync(string name)
        {
            var entity = await base.FirstOrDefaultAsync(x => x.Name == name);
            return _mapper.Map<WebsiteDomainModel>(entity);
        }

        public async Task<IEnumerable<WebsiteDomainModel>> SearchAsync(Expression<Func<WebsiteDomainModel, bool>> predicate = null)
        {
            var list = await GetByQueryAsync(x => true);
            return _mapper.Map<List<WebsiteDomainModel>>(list);
        }
    }
}
