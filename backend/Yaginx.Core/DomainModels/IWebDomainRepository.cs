namespace Yaginx.DomainModels;

public interface IWebDomainRepository
{
    Task<IEnumerable<WebDomain>> SearchAsync(bool useFreeCert = false);
    Task AddAsync(WebDomain webDomain);
    Task UpdateAsync(WebDomain webDomain);
    Task DeleteAsync(long id);
    Task<WebDomain> GetAsync(long id);
    Task<WebDomain> GetByNameAsync(string name);
}
