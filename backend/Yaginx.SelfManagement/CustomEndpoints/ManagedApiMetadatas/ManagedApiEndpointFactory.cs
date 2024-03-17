using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;

namespace Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;
public sealed class ManagedApiEndpointFactory
{
    private readonly YaginxConsoleOption _yaginxConsoleOption;
    private RequestDelegate _pipeline;

    public ManagedApiEndpointFactory(YaginxConsoleOption yaginxConsoleOption)
    {
        _yaginxConsoleOption = yaginxConsoleOption;
    }
    public Endpoint CreateManagedApiEndpoint(string name, ManagedApiMetadataModel model, int order = 0, IReadOnlyList<Action<EndpointBuilder>> conventions = null)
    {
        var pathPattern = $"{_yaginxConsoleOption.ConsolePath}/{{**catchall}}";

        var endpointBuilder = new RouteEndpointBuilder(
            requestDelegate: _pipeline ?? throw new InvalidOperationException("The pipeline hasn't been provided yet."),
            routePattern: RoutePatternFactory.Parse(pathPattern),
            order: order)
        {
            DisplayName = name
        };

        endpointBuilder.Metadata.Add(model);

        var brand = model.Brand;
        var group = model.Group;
        var serviceName = model.ServiceName;
        var serviceType = model.ServiceType;

        var matchers = new List<ManagedApiUrlRuleMetadataMatcher>(1)
        {
            new ManagedApiUrlRuleMetadataMatcher(brand, group, serviceType, serviceName)
        };
        endpointBuilder.Metadata.Add(new ManagedApiUrlRuleMetadata(matchers));

        //endpointBuilder.Metadata.Add(new AuthorizeAttribute(GlobalConsts.AuthorizationPolicy_ManagedApi));

        for (var i = 0; i < conventions.Count; i++)
        {
            conventions[i](endpointBuilder);
        }

        return endpointBuilder.Build();
    }
    public void SetProxyPipeline(RequestDelegate pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }
}