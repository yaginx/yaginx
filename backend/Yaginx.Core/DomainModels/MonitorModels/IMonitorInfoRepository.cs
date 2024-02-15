namespace Yaginx.DomainModels.MonitorModels;

public interface IMonitorInfoRepository
{
    Task AddAsync(ResourceMonitorInfo monitorInfoEntity);    
}
public interface IResourceReportRepository
{
    Task<List<ResourceReportModel>> SearchAsync(ReportSearchRequest reportSearchRequest);
}
