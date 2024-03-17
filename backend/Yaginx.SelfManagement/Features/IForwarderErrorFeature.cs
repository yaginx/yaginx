namespace Yaginx.SelfManagement.Features
{
    public interface IForwarderErrorFeature
    {
        /// <summary>
        /// The specified ProxyError.
        /// </summary>
        ManagedApiError Error { get; }

        /// <summary>
        /// An Exception that occurred when forwarding the request to the destination, if any.
        /// </summary>
        Exception Exception { get; }
    }
}
