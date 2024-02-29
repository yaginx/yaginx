using AutoMapper;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class UserRepository : YaginxCrudBaseRepository<User, UserEntity>, IUserRepository
    {
        public UserRepository(IWoDbContextFactory factory, IMapper mapper, ILogger<UserRepository> logger) : base(factory, mapper, logger)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var entity = await base.GetAsync(x => x.Email == email);
            return _mapper.Map<User>(entity);
        }
    }
}
