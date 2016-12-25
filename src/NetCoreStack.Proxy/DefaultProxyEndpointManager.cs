using System;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class DefaultProxyEndpointManager : IProxyEndpointManager
    {
        protected RoundRobinManager RoundRobinManager { get; }

        public DefaultProxyEndpointManager(RoundRobinManager roundRobinManager)
        {
            RoundRobinManager = roundRobinManager;
        }

        public UriBuilder CreateUriBuilder(ProxyDescriptor descriptor, string regionKey, string targetMethodName)
        {
            var uriBuilder = RoundRobinManager.RoundRobinUri(regionKey);

            if (uriBuilder == null)
            {
                throw new ArgumentNullException(nameof(uriBuilder));
            }

            if (!string.IsNullOrEmpty(descriptor.Route))
            {
                if (targetMethodName.ToLower() == HttpMethod.Get.Method.ToLower())
                    uriBuilder.Path = $"{descriptor.Route}/";
                else
                    uriBuilder.Path = $"{descriptor.Route}/{targetMethodName}";
            }

            return uriBuilder;
        }
    }
}
