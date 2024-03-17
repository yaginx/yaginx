namespace Yaginx.SelfManagement.Features;

public static class ManagedApiLogFeatureExtensions
{
    public static void AddLogMetadata(this IManagedApiLogFeature feature, string key, string value)
    {
        feature.Metadata.Add(key, value);
    }

    public static void AddLogTag(this IManagedApiLogFeature feature, string key, string value)
    {
        feature.Tags.Add(key, value);
    }

    public static IManagedApiLogFeature GetLogFeature(this HttpContext httpContext)
    {
        return httpContext.Features.Get<IManagedApiLogFeature>();
    }
}
