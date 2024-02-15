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
        public void Add(User user)
        {
            _databaseRepository.Insert(user);
        }

        public int Count()
        {
            return _databaseRepository.Search<User>().Count();
        }

        public User GetByEmail(string email)
        {
            return _databaseRepository.Get<User>(x => x.Email == email);
        }

        public void Update(User user)
        {
            _databaseRepository.Update(user.Id, user);
        }
    }
}
