namespace Yaginx.DomainModels;

public class PeriodInfo
{
    public long RequestTs { get; set; }
    public long PeriodTs { get; set; }
    public Dictionary<string, object> Items { get; set; } = new Dictionary<string, object>();
    public SeqNoResetPeriod Period { get; set; }
}
