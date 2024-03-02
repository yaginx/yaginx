using AgileLabs;
using AgileLabs.Securities;
using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DomainModels;

namespace Yaginx.HostedServices
{
    public class InitService : IScoped
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IDbContextCommiter _dbContextCommiter;

        public InitService(IUserRepository userRepository, IEncryptionService encryptionService, IDbContextCommiter dbContextCommiter)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _dbContextCommiter = dbContextCommiter;
        }
        internal async Task Init()
        {
            var userCount = await _userRepository.CountAsync();
            if (userCount < 1)
            {
                var userInfo = new User
                {
                    Id = IdGenerator.NextId(),
                    Email = "admin@yaginx.com",
                    Password = "admin",
                    PasswordSalt = _encryptionService.CreateSaltKey(16)
                };
                userInfo.PasswordHash = _encryptionService.CreatePasswordHash(userInfo.Password, userInfo.PasswordSalt, "SHA256");
                await _userRepository.AddAsync(userInfo);
                await _dbContextCommiter.CommitAsync();
                await Task.CompletedTask;
            }
        }
    }
}