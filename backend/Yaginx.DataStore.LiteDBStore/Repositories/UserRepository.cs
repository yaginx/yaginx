using Yaginx.DomainModels;

namespace Yaginx.DataStore.LiteDBStore.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LiteDbDatabaseRepository _databaseRepository;

        public UserRepository(LiteDbDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }
        public async Task AddAsync(User user)
        {
            await _databaseRepository.InsertAsync(user);
        }

        public async Task<int> CountAsync()
        {
            var result = await _databaseRepository.SearchAsync<User>();
            return result.Count();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _databaseRepository.GetAsync<User>(x => x.Email == email);
        }

        public async Task UpdateAsync(User user)
        {
            await _databaseRepository.UpdateAsync(user.Id, user);
        }
    }
}
