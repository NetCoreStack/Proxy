using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Contracts;
using System;
using System.Reflection;

namespace NetCoreStack.Proxy.Internal
{
    internal static class ProxyHelper
    {
        internal static TProxy CreateProxy<TProxy>(IServiceProvider container) where TProxy : IApiContract
        {
            dynamic proxy = DispatchProxyAsync.Create<TProxy, HttpDispatchProxy>();
            var proxyContextFilter = container.GetRequiredService<IProxyContextFilter>();
            var proxyContext = new ProxyContext(typeof(TProxy));

            var cultureFactory = container.GetService<ICultureFactory>();
            if (cultureFactory != null)
            {
                proxyContext.CultureFactory = cultureFactory.Invoke;
            }

            proxyContextFilter.Invoke(proxyContext);
            proxy.Initialize(proxyContext, container.GetService<IProxyManager>());
            return proxy;            
        }
    }
}