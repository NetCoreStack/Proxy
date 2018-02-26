using System;
using System.Globalization;

namespace NetCoreStack.Proxy.Internal
{
    public class ProxyContext
    {
        public string ClientIp { get; set; }
        public string Query { get; set; }
        public string UserAgent { get; set; }
        public Type ProxyType { get; }
        public Func<CultureInfo> CultureFactory { get; set; }

        public ProxyContext(Type proxyType)
        {
            ProxyType = proxyType;
        }
    }
}
