namespace Yaginx.DomainModels.MonitorModels;

public class ResourceReportModel
{
    public string ResourceUuid { get; set; }    
    public ReportCycleType CycleType { get; set; }
    public long ReportTime { get; set; }

    /// <summary>
    /// 总处理请求
    /// </summary>
    public long RequestQty { get; set; }
    public List<KeyValuePair<string, long>> StatusCode { get; set; }
    public List<KeyValuePair<string, long>> Spider { get; set; }
    public List<KeyValuePair<string, long>> Browser { get; set; }
    public List<KeyValuePair<string, long>> Os { get; set; }
    public List<KeyValuePair<string, long>> Duration { get; set; }
}
