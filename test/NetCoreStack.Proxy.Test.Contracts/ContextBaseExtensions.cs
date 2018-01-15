using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public static class ContextBaseExtensions
    {
        public readonly static string ClientUserAgentHeader = "User-Agent";

        public static string GetIp(this HttpContext context)
        {
            return context?.Features?.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
        }

        public static string GetUserAgent(this HttpRequest request)
        {
            var userAgent = request.Headers[ClientUserAgentHeader].ToString();
            if (!string.IsNullOrEmpty(userAgent))
            {
                return userAgent;
            }

            return string.Empty;
        }
    }
}
