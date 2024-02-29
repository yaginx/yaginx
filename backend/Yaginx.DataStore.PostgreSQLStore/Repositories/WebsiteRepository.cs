using AutoMapper;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class WebsiteRepository : YaginxCrudBaseRepository<Website, WebsiteEntity>, IWebsiteRepository
    {
        public WebsiteRepository(IWoDbContextFactory factory, IMapper mapper, ILogger<WebsiteRepository> logger) : base(factory, mapper, logger)
        {
        }

        public async Task<Website> GetByNameAsync(string name)
        {
            var entity = await base.GetAsync(x => x.Name == name);
            return _mapper.Map<Website>(entity);
        }
    }
}
