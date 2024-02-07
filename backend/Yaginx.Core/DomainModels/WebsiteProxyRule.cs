namespace Yaginx.DomainModels
{
    public class WebsiteProxyRule
    {
        public string PathPattern { get; set; }
        public List<WebsiteDestination> Destinations { get; set; }

    }
}
