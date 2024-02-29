namespace Yaginx.DomainModels;

public interface IUserRepository
{
    Task<long> CountAsync();
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
