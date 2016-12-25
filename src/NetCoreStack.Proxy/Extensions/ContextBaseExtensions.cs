using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NetCoreStack.Proxy.Internal;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class ContextBaseExtensions
    {
        internal static string GetIp(this HttpContext context)
        {
            return context?.Features?.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
        }

        internal static string GetUserAgent(this HttpRequest request)
        {
            var userAgent = request.Headers[Constants.ClientUserAgentHeader].ToString();
            if (userAgent.HasValue())
            {
                return userAgent;
            }

            return string.Empty;
        }
    }
}
