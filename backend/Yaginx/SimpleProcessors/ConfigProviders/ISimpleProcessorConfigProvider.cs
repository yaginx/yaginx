namespace Yaginx.SimpleProcessors.ConfigProviders;

public interface ISimpleProcessorConfigProvider
{
    /// <summary>
    /// Returns the current route and cluster data.
    /// </summary>
    /// <returns></returns>
    ISimpleProcessorConfig GetConfig();
}
