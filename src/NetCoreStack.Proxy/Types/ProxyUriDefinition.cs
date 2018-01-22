using System;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class ProxyUriDefinition
    {
        public static readonly IDictionary<string, string> TemplateCache =
            new Dictionary<string, string>();

        public UriBuilder UriBuilder { get; set; }

        public bool HasParameter { get; private set; }

        public ProxyUriDefinition(UriBuilder uriBuilder)
        {
            UriBuilder = uriBuilder;
        }

        public void ResolveTemplate(ProxyMethodDescriptor methodDescriptor, string route, string template)
        {
            var path = UriBuilder.Path ?? string.Empty;
            if (methodDescriptor.RouteTemplate != null)
            {
                if (methodDescriptor.RouteTemplate.Parameters.Count > 0)
                {
                    HasParameter = true;
                    var key = route + "/" + template;
                    if (TemplateCache.TryGetValue(key, out string tmpl))
                    {
                        path += $"{route}/{tmpl}";
                        UriBuilder.Path = path;
                        return;
                    }

                    tmpl = string.Join("/", methodDescriptor.TemplateKeys);
                    TemplateCache.Add(key, tmpl);

                    path += $"{route}/{tmpl}";
                    UriBuilder.Path = path;
                    return;
                }
            }

            path += $"{route}/{template}";
            UriBuilder.Path = path;
        }
    }
}
