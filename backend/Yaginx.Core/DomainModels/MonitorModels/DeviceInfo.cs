namespace Yaginx.DomainModels.MonitorModels;

public class DeviceInfo
{
    //
    // 摘要:
    //     Returns true if the device is likely to be a spider or a bot device
    public bool IsSpider { get; set; }
    //
    // 摘要:
    //     The brand of the device
    public string Brand { get; set; }
    //
    // 摘要:
    //     The family of the device, if available
    public string Family { get; set; }
    //
    // 摘要:
    //     The model of the device, if available
    public string Model { get; set; }
}
