using System.Linq.Expressions;

namespace Yaginx.DomainModels;

public interface IWebsiteRepository
{
    Task<IEnumerable<WebsiteDomainModel>> SearchAsync(Expression<Func<WebsiteDomainModel, bool>> predicate = null);
    Task AddAsync(WebsiteDomainModel website);
    Task UpdateAsync(WebsiteDomainModel website);
    Task DeleteAsync(long id);
    Task<WebsiteDomainModel> GetAsync(long id);
    Task<WebsiteDomainModel> GetByNameAsync(string name);
}
