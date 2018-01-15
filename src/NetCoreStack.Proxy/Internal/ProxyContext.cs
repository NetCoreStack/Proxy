using System;

namespace NetCoreStack.Proxy
{
    public class ProxyContext
    {
        public string ClientIp { get; set; }
        public string Query { get; set; }
        public string UserAgent { get; set; }
        public Type ProxyType { get; }

        public ProxyContext(Type proxyType)
        {
            ProxyType = proxyType;
        }
    }
}
