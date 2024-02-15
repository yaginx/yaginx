namespace Yaginx.DomainModels;

public interface IHostTrafficRepository
{
    Task UpsertAsync(HostTraffic hostTraffic);
    Task<List<HostTraffic>> SearchAsync();
    Task<List<HostTraffic>> SearchAsync(string hostName);
}
