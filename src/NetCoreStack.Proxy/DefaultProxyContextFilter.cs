using System.Net;
using System.Net.Sockets;

namespace NetCoreStack.Proxy
{
    public class DefaultProxyContextFilter : IProxyContextFilter
    {
        public const string NetCoreStackUserAgent = "NetCoreStack-Proxy-UserAgent";
        public readonly static string LocalIpAddress = GetLocalIPAddress();

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

        public void Invoke(ProxyContext proxyContext)
        {
            proxyContext.ClientIp = LocalIpAddress;
            proxyContext.UserAgent = NetCoreStackUserAgent;
        }
    }
}