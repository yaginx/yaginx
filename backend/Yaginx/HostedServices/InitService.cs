using AgileLabs;
using AgileLabs.Securities;
using Yaginx.DomainModels;

namespace Yaginx.HostedServices
{
    public class InitService : IScoped
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;

        public InitService(IUserRepository userRepository, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }
        internal async Task Init()
        {
            var userCount = _userRepository.Count();
            if (userCount <1)
            {
                var userInfo = new User
                {
                    Id = IdGenerator.NextId(),
                    Email = "admin@yaginx.com",
                    Password = "admin",
                    PasswordSalt = _encryptionService.CreateSaltKey(16)
                };
                userInfo.PasswordHash = _encryptionService.CreatePasswordHash(userInfo.Password, userInfo.PasswordSalt, "SHA256");
                _userRepository.Add(userInfo);
                await Task.CompletedTask;
            }
        }
    }
}