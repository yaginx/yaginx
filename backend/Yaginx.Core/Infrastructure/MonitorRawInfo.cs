using System.Runtime.Serialization;

namespace Yaginx.Infrastructure;

public class MonitorRawInfo
{
    [DataMember(Name = "h")]
    public string Host { get; set; }
    [DataMember(Name = "p")]
    public string Path { get; set; }
    [DataMember(Name = "qs")]
    public string QueryString { get; set; }
    [DataMember(Name = "r")]
    public string Referer { get; set; }
    [DataMember(Name = "ua")]
    public string UserAgent { get; set; }
    [DataMember(Name = "ip")]
    public string Ip { get; set; }
    [DataMember(Name = "l")]
    public string Lang { get; set; }
    [DataMember(Name = "sc")]
    public int StatusCode { get; set; }
    [DataMember(Name = "d")]
    public long Duration { get; set; }
}