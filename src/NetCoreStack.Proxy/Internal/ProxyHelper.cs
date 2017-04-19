using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Extensions;
using System;
using System.Reflection;

namespace NetCoreStack.Proxy.Internal
{
    internal static class ProxyHelper
    {
        public static TProxy CreateProxy<TProxy>(IServiceProvider container) where TProxy : IApiContract
        {
            dynamic proxy = DispatchProxyAsync.Create<TProxy, HttpDispatchProxy>();
            var contextAcessor = container.GetService<IHttpContextAccessor>();

            if (contextAcessor?.HttpContext == null)
                return default(TProxy);

            var context = new ProxyContext(typeof(TProxy));
            if (contextAcessor?.HttpContext.Request != null)
            {
                context.ClientIp = contextAcessor.HttpContext.GetIp();
                context.UserAgent = contextAcessor.HttpContext.Request.GetUserAgent();
                if (contextAcessor.HttpContext.Request.QueryString.HasValue)
                {
                    context.QueryString = contextAcessor.HttpContext.Request.QueryString.Value;
                }
            }

            proxy.Initialize(context, container.GetService<IProxyManager>());
            return proxy;
        }
    }
}
