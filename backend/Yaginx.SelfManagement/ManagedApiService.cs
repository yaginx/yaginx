using Yaginx.SelfManagement.Features;

namespace Yaginx.SelfManagement
{
    public abstract class ManagedApiService : IManagedApiService
    {
        public HttpContext HttpContext { get; set; }
        protected IManagedApiLogFeature LogFeature => HttpContext.GetLogFeature();

        public void AddLogMetadata(string key, string value) => LogFeature.AddLogMetadata(key, value);

        public void AddLogTag(string key, string value) => LogFeature.AddLogTag(key, value);
    }
}
