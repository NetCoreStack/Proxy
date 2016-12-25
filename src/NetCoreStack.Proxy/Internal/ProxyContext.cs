using System;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class ProxyContext
    {
        public string ClientIp { get; set; }
        public string QueryString { get; set; }
        public string UserAgent { get; set; }
        public Type ProxyType { get; }

        public KeyValuePair<string, string> TokenCookie { get; set; }

        public ProxyContext(Type proxyType)
        {
            ProxyType = proxyType;
        }
    }
}
