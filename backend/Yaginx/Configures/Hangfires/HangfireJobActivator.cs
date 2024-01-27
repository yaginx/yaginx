using AgileLabs;
using Hangfire;

namespace Yaginx.Configures.Hangfires;
public class HangfireJobActivator : JobActivator
{
	private readonly IServiceProvider _serviceProvider;

	public HangfireJobActivator(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
	}
	public override JobActivatorScope BeginScope(JobActivatorContext context)
	{
		return new WorkContextJobActivatorScope(AgileLabContexts.Context.RootServiceProvider.CreateScope());
	}
}