using Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;
using Yarp.ReverseProxy.Model;

namespace Yaginx.SimpleProcessors;

public interface ISimpleProcessorFeature
{
    WebsitePreProcessMetadataModel Model { get; set; }
}
public class SimpleProcessorFeature : ISimpleProcessorFeature
{
    public WebsitePreProcessMetadataModel Model { get;  set; }
}
