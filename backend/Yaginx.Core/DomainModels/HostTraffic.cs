namespace Yaginx.DomainModels;

public class HostTraffic
{
    public long Id { get; set; }
    public string HostName { get; set; }
    public long Period { get; set; }
    public long RequestCounts { get; set; }
    public long InboundBytes { get; set; }
    public long OutboundBytes { get; set; }
}
