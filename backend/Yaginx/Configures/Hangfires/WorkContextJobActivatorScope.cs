using Hangfire;
namespace Yaginx.Configures.Hangfires;
public class WorkContextJobActivatorScope : JobActivatorScope
{
	private readonly IServiceScope _serviceProvider;

	public WorkContextJobActivatorScope(IServiceScope serviceScope)
	{
		_serviceProvider = serviceScope;
	}

	public override object Resolve(Type type)
	{
		return _serviceProvider.ServiceProvider.GetService(type);
	}

	public override void DisposeScope()
	{
		_serviceProvider.Dispose();
	}
}