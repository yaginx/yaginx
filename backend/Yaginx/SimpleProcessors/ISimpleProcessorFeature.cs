using Yarp.ReverseProxy.Model;

namespace Yaginx.SimpleProcessors;

public interface ISimpleProcessorFeature
{

}
public class SimpleProcessorFeature : ISimpleProcessorFeature
{
    //private IReadOnlyList<DestinationState> _availableDestinations = default!;

    ///// <inheritdoc/>
    //public RouteModel Route { get; init; } = default!;

    ///// <inheritdoc/>
    //public ClusterModel Cluster { get; set; } = default!;

    ///// <inheritdoc/>
    //public IReadOnlyList<DestinationState> AllDestinations { get; init; } = default!;

    ///// <inheritdoc/>
    //public IReadOnlyList<DestinationState> AvailableDestinations
    //{
    //    get => _availableDestinations;
    //    set => _availableDestinations = value ?? throw new ArgumentNullException(nameof(value));
    //}

    ///// <inheritdoc/>
    //public DestinationState? ProxiedDestination { get; set; }
}
