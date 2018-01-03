using Microsoft.AspNetCore.Http;
using NetCoreStack.Proxy.Test.Contracts;

namespace NetCoreStack.Proxy.Tests
{
    public class CustomProxyContextFilter : IProxyContextFilter
    {
        private readonly IHttpContextAccessor contextAccessor;

        public CustomProxyContextFilter(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public void Invoke(ProxyContext proxyContext)
        {
            if (contextAccessor?.HttpContext == null)
            {
                return;
            }

            if (contextAccessor?.HttpContext.Request != null)
            {
                proxyContext.ClientIp = contextAccessor.HttpContext.GetIp();
                proxyContext.UserAgent = contextAccessor.HttpContext.Request.GetUserAgent();
                if (contextAccessor.HttpContext.Request.QueryString.HasValue)
                {
                    proxyContext.Query = contextAccessor.HttpContext.Request.QueryString.Value;
                }
            }
        }
    }
}
