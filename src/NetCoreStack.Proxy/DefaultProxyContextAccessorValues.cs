using System;
using System.Net;
using System.Net.Sockets;

namespace NetCoreStack.Proxy
{
    public class DefaultProxyContextAccessorValues : IProxyContextAccessor
    {
        public const string NetCoreStackUserAgent = "NetCoreStack-Proxy-UserAgent";
        public readonly static string LocalIpAddress = GetLocalIPAddress();
        private readonly Type proxyType;

        private ProxyContext _proxyContext;
        public ProxyContext ProxyContext
        {
            get
            {
                if (_proxyContext == null)
                {
                    _proxyContext = new ProxyContext(proxyType)
                    {
                        ClientIp = LocalIpAddress,
                        UserAgent = NetCoreStackUserAgent
                    };
                }

                return _proxyContext;
            }
            set { }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return string.Empty;
        }

        public DefaultProxyContextAccessorValues(Type proxyType)
        {
            this.proxyType = proxyType;
        }
    }
}