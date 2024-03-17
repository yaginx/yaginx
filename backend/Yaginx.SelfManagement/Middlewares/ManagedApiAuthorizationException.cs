using System.Runtime.Serialization;

namespace Yaginx.SelfManagement.Middlewares
{
    [Serializable]
    internal class ManagedApiAuthorizationException : Exception
    {
        public ManagedApiAuthorizationException()
        {
        }

        public ManagedApiAuthorizationException(string message) : base(message)
        {
        }

        public ManagedApiAuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}