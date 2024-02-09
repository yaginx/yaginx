using AgileLabs.WorkContexts;

namespace Yaginx.WorkContexts;

public class DefaultWorkContext : DefaultWorkContextCore, IWorkContext, IWorkContextSetter
{

    public static DefaultWorkContext CreateDefault()
    {
        var workContext = new DefaultWorkContext();
        return workContext;
    }
}
