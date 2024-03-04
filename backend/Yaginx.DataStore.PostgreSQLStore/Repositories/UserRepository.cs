using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class UserRepository : YaginxCrudBaseRepository<User, AccountEntity>, IUserRepository
    {
        public UserRepository(IAgileLabDbContextFactory factory, IMapper mapper, ILogger<UserRepository> logger) : base(factory, mapper, logger)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var entity = await base.GetAsync(x => x.Email == email);
            return _mapper.Map<User>(entity);
        }
    }
}
