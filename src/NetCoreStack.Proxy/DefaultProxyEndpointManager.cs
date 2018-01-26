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

        private string ConcatRoute(string route, string methodName)
        {
            string path = string.Empty;
            if (string.IsNullOrEmpty(route))
            {
                path = methodName;
            }
            else
            {
                path = $"{route}/{methodName}";
            }

            return path;
        }

        private void ResolveTemplate(UriBuilder uriBuilder, ProxyMethodDescriptor methodDescriptor, string route, string targetMethodName)
        {
            var path = uriBuilder.Path ?? string.Empty;
            if (methodDescriptor.RouteTemplate != null)
            {
                if (methodDescriptor.RouteTemplate.Parameters.Count > 0)
                {
                    var methodName = string.Join("/", methodDescriptor.TemplateKeys);
                    path += ConcatRoute(route, methodName);
                    uriBuilder.Path = path;
                    return;
                }
            }

            path += ConcatRoute(route, targetMethodName);
            uriBuilder.Path = path;
        }

        public UriBuilder CreateUriBuilder(ProxyMethodDescriptor methodDescriptor, string route, string regionKey, string targetMethodName)
        {
            var uriBuilder = RoundRobinManager.RoundRobinUri(regionKey);
            if (uriBuilder == null)
            {
                throw new ArgumentNullException(nameof(uriBuilder));
            }

            if (targetMethodName.ToLower() == HttpMethod.Get.Method.ToLower())
            {
                var path = uriBuilder.Path ?? string.Empty;                
                path += string.IsNullOrEmpty(route) ? "" : $"{route}/";
                uriBuilder.Path = path;
            }
            else
            {
                if (targetMethodName.StartsWith("/"))
                    targetMethodName = targetMethodName.Substring(1);

                ResolveTemplate(uriBuilder, methodDescriptor, route, targetMethodName);
            }

            return uriBuilder;
        }
    }
}