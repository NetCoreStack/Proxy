using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Contracts;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace NetCoreStack.Proxy.Internal
{
    internal static class ProxyHelper
    {
        private const string NetCoreStackUserAgent = "NetCoreStack-Proxy-UserAgent";
        private readonly static string LocalIpAddress = GetLocalIPAddress();

        private static string GetLocalIPAddress()
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

        public static TProxy CreateProxy<TProxy>(IServiceProvider container) where TProxy : IApiContract
        {
            dynamic proxy = DispatchProxyAsync.Create<TProxy, HttpDispatchProxy>();
            var contextAcessor = container.GetService<IProxyContextAccessor>();
            if (contextAcessor.ProxyContext != null)
            {
                proxy.Initialize(contextAcessor.ProxyContext, container.GetService<IProxyManager>());
                return proxy;
            }

            var context = new ProxyContext(typeof(TProxy));
            context.UserAgent = NetCoreStackUserAgent;
            context.ClientIp = LocalIpAddress;
            proxy.Initialize(context, container.GetService<IProxyManager>());
            return proxy;            
        }
    }
}