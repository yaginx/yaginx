using Yaginx.SimpleProcessors.ConfigProviders;

namespace Yaginx.SimpleProcessors;

public interface ISimpleProcessorConfigChangeListener
{
    /// <summary>
    /// Invoked when an error occurs while loading the configuration.
    /// </summary>
    /// <param name="configProvider">The instance of the configuration provider that failed to provide the configuration.</param>
    /// <param name="exception">The thrown exception.</param>
    void ConfigurationLoadingFailed(ISimpleProcessorConfigProvider configProvider, Exception exception);

    /// <summary>
    /// Invoked once the configuration have been successfully loaded.
    /// </summary>
    /// <param name="proxyConfigs">The list of instances that have been loaded.</param>
    void ConfigurationLoaded(IReadOnlyList<ISimpleProcessorConfig> proxyConfigs);

    /// <summary>
    /// Invoked when an error occurs while applying the configuration.
    /// </summary>
    /// <param name="proxyConfigs">The list of instances that were being processed.</param>
    /// <param name="exception">The thrown exception.</param>
    void ConfigurationApplyingFailed(IReadOnlyList<ISimpleProcessorConfig> proxyConfigs, Exception exception);

    /// <summary>
    /// Invoked once the configuration has been successfully applied.
    /// </summary>
    /// <param name="proxyConfigs">The list of instances that have been applied.</param>
    void ConfigurationApplied(IReadOnlyList<ISimpleProcessorConfig> proxyConfigs);
}
