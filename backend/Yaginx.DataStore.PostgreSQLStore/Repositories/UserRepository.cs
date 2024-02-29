using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CrudRepository<User> _userCrudRepository;

        public UserRepository(CrudRepository<User> userCrudRepository)
        {
            _userCrudRepository = userCrudRepository;
        }
        public async Task AddAsync(User user)
        {
            await _userCrudRepository.InsertAsync(user);
        }

        public async Task<int> CountAsync()
        {
            var result = await _userCrudRepository.CountAsync(x => true);
            return result;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userCrudRepository.GetAsync<User>(x => x.Email == email);
        }

        public async Task UpdateAsync(User user)
        {
            await _userCrudRepository.SingleUpdateAsync(user);
        }
    }
}
