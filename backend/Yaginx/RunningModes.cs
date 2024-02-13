using AgileLabs.Infrastructure;
using AgileLabs.WebApp.Hosting;

public static class RunningModes
{
    internal const string Key = "RUNNING_MODE";
    public static RunningMode RunningMode => Enum.Parse<RunningMode>(Singleton<AppBuildContext>.Instance.Items[Key].ToString());
}
