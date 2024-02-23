using LettuceEncrypt.Acme;
using LettuceEncrypt.Internal.AcmeStates;
using LettuceEncrypt.Internal.IO;
using LettuceEncrypt.Internal.PfxBuilder;
using LettuceEncrypt.Internal;
using LettuceEncrypt;
using McMaster.AspNetCore.Kestrel.Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Yaginx.SimpleProcessors;
using Yaginx.SimpleProcessors.RequestMetadatas;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Helper methods for configuring Lettuce Encrypt services.
/// </summary>
public static class SimpleProcessorServiceCollectionExtensions
{
    public static ISimpleProcessorServiceBuilder AddSimpleProcessor(this IServiceCollection services)
    {
        services.TryAddSingleton<SimpleProcessorConfigManager>();
        services.TryAddSingleton<SimpleProcessorEndpointFactory>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, RequestMetadataMatchPolicy>());
        return new SimpleProcessorServiceBuilder(services);
    }

    public interface ISimpleProcessorServiceBuilder
    {

    }
    public class SimpleProcessorServiceBuilder : ISimpleProcessorServiceBuilder
    {
        public SimpleProcessorServiceBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}

