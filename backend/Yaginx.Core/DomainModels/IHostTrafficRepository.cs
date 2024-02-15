namespace Yaginx.DomainModels;

public interface IHostTrafficRepository
{
    Task Upsert(HostTraffic hostTraffic);
    Task<List<HostTraffic>> Search();
    Task<List<HostTraffic>> Search(string hostName);
}
