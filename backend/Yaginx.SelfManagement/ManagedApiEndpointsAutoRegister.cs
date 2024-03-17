using AgileLabs;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Routing;

namespace Yaginx.SelfManagement
{
    public class ManagedApiEndpointsAutoRegister : IEndpointConfig
    {
        public void ConfigureEndpoints(IEndpointRouteBuilder endpoints, AppBuildContext appBuildContext)
        {
            endpoints.MapManagedApi();
        }
    }
}
