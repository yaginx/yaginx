using Yaginx.SimpleProcessors.ConfigProviders;
using Yarp.ReverseProxy.Model;

namespace Yaginx.SimpleProcessors;

public interface ISimpleProcessorFeature
{
    RequestMetadataModel Model { get; set; }
}
public class SimpleProcessorFeature : ISimpleProcessorFeature
{
    public RequestMetadataModel Model { get;  set; }
}
