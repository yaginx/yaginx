namespace Yaginx.SimpleProcessors;

public class SimpleProcessorConventionBuilder : IEndpointConventionBuilder
{
    private readonly List<Action<EndpointBuilder>> _conventions;

    internal SimpleProcessorConventionBuilder(List<Action<EndpointBuilder>> conventions)
    {
        _conventions = conventions ?? throw new ArgumentNullException(nameof(conventions));
    }

    /// <summary>
    /// Adds the specified convention to the builder. Conventions are used to customize <see cref="EndpointBuilder"/> instances.
    /// </summary>
    /// <param name="convention">The convention to add to the builder.</param>
    public void Add(Action<EndpointBuilder> convention)
    {
        _ = convention ?? throw new ArgumentNullException(nameof(convention));

        _conventions.Add(convention);
    }
}
