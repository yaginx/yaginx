namespace Yaginx.DomainModels.MonitorModels;

public class ReportSearchRequest
{
    public string ResourceUuid { get; set; }
    public ReportCycleType CycleType { get; set; }
    public long BeginTime { get; set; }
    public long EndTime { get; set; }
}
