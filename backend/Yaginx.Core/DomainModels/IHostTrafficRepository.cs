using AgileLabs.ComponentModels;
using AgileLabs.DynamicSearch;

namespace Yaginx.DomainModels;

public interface IHostTrafficRepository
{
    Task UpsertAsync(HostTraffic hostTraffic);
    Task<Page<HostTraffic>> SearchAsync(SearchParameters searchParameters);
    Task<IEnumerable<HostTraffic>> SearchAsync(string hostName);
}
