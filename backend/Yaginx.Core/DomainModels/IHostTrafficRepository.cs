namespace Yaginx.DomainModels;

public interface IHostTrafficRepository
{
    Task UpsertAsync(HostTraffic hostTraffic);
    Task<IEnumerable<HostTraffic>> SearchAsync();
    Task<IEnumerable<HostTraffic>> SearchAsync(string hostName);
}
