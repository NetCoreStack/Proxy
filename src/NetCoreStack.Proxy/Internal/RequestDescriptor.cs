using NetCoreStack.Proxy.Extensions;
using System;
using System.Net.Http;

namespace NetCoreStack.Proxy.Internal
{
    public class RequestDescriptor
    {
        public ProxyMethodDescriptor MethodDescriptor { get; }
        public HttpRequestMessage Request { get; }
        public string RegionKey { get; }
        public TimeSpan? Timeout { get; }

        public RequestDescriptor(HttpRequestMessage request,
            ProxyMethodDescriptor methodDescriptor,
            string regionKey,
            TimeSpan? timeout = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (methodDescriptor == null)
            {
                throw new ArgumentNullException(nameof(methodDescriptor));
            }

            if (!regionKey.HasValue())
            {
                throw new ArgumentNullException(nameof(regionKey));
            }

            Request = request;
            MethodDescriptor = methodDescriptor;
            RegionKey = regionKey;
            Timeout = timeout;
        }
    }
}
