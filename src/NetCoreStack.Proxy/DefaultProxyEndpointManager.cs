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

        public ProxyUriDefinition CreateUriDefinition(ProxyDescriptor descriptor, string regionKey, string targetMethodName)
        {
            var uriBuilder = RoundRobinManager.RoundRobinUri(regionKey);
            if (uriBuilder == null)
            {
                throw new ArgumentNullException(nameof(uriBuilder));
            }

            var uriDefinition = new ProxyUriDefinition(uriBuilder);

            if (!string.IsNullOrEmpty(descriptor.Route))
            {
                if (targetMethodName.ToLower() == HttpMethod.Get.Method.ToLower())
                {
                    var path = uriDefinition.UriBuilder.Path ?? string.Empty;
                    path += $"{descriptor.Route}/";
                    uriDefinition.UriBuilder.Path = path;
                }   
                else
                {
                    if (targetMethodName.StartsWith("/"))
                        targetMethodName = targetMethodName.Substring(1);

                    var routeTemplate = TemplateParser.Parse(targetMethodName);
                    uriDefinition.ResolveTemplate(routeTemplate, descriptor.Route, targetMethodName);
                }
            }

            return uriDefinition;
        }
    }
}
