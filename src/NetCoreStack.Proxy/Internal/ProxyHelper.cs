using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Contracts;
using System;
using System.Reflection;

namespace NetCoreStack.Proxy.Internal
{
    internal static class ProxyHelper
    {
        public static TProxy CreateProxy<TProxy>(IServiceProvider container) where TProxy : IApiContract
        {
            dynamic proxy = DispatchProxyAsync.Create<TProxy, HttpDispatchProxy>();
            var contextAcessor = container.GetService<IProxyContextAccessor>();
            if (contextAcessor.ProxyContext != null)
            {
                proxy.Initialize(contextAcessor.ProxyContext, container.GetService<IProxyManager>());
                return proxy;
            }

            contextAcessor = new DefaultProxyContextAccessorValues(typeof(TProxy));
            proxy.Initialize(contextAcessor.ProxyContext, container.GetService<IProxyManager>());
            return proxy;            
        }
    }
}