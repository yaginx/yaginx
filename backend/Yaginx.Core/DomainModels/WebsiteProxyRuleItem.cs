namespace Yaginx.DomainModels;

public class WebsiteProxyRuleItem
{
    public string PathPattern { get; set; }
    public List<WebsiteDestination> Destinations { get; set; } = new List<WebsiteDestination>();
}
