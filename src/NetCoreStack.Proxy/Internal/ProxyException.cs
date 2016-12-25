using System;

namespace NetCoreStack.Proxy
{
    public class ProxyException : Exception
    {
        public ProxyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
