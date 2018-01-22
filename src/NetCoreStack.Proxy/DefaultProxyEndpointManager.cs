using Microsoft.AspNetCore.Routing.Template;
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

        public ProxyUriDefinition CreateUriDefinition(ProxyMethodDescriptor methodDescriptor, string route, string regionKey, string targetMethodName)
        {
            var uriBuilder = RoundRobinManager.RoundRobinUri(regionKey);
            if (uriBuilder == null)
            {
                throw new ArgumentNullException(nameof(uriBuilder));
            }

            var uriDefinition = new ProxyUriDefinition(uriBuilder);

            if (!string.IsNullOrEmpty(route))
            {
                if (targetMethodName.ToLower() == HttpMethod.Get.Method.ToLower())
                {
                    var path = uriDefinition.UriBuilder.Path ?? string.Empty;
                    path += $"{route}/";
                    uriDefinition.UriBuilder.Path = path;
                }
                else
                {
                    if (targetMethodName.StartsWith("/"))
                        targetMethodName = targetMethodName.Substring(1);

                    uriDefinition.ResolveTemplate(methodDescriptor, route, targetMethodName);
                }
            }

            return uriDefinition;
        }
    }
}