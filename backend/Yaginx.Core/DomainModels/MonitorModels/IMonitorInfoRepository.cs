

namespace Yaginx.DomainModels.MonitorModels;

public interface IMonitorInfoRepository
{
    Task AddAsync(ResourceMonitorInfo monitorInfoEntity);
    Task<IList<ResourceMonitorInfo>> SearchAsync(DateTime beginTime, DateTime endTime);
}
public interface IResourceReportRepository
{
    Task<List<ResourceReportModel>> SearchAsync(ReportSearchRequest reportSearchRequest);
    Task<List<ResourceReportModel>> SearchAsync(ReportCycleType hourly, DateTime beginTime, DateTime endTime);
    Task UpsertAsync(ResourceReportModel resourceReport);
}
