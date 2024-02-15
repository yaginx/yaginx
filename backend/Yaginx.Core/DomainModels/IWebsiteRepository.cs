namespace Yaginx.DomainModels;

public interface IWebsiteRepository
{
    Task<IEnumerable<Website>> SearchAsync();
    Task AddAsync(Website website);
    Task UpdateAsync(Website website);
    Task DeleteAsync(long id);
    Task<Website> GetAsync(long id);
    Task<Website> GetByNameAsync(string name);
}
