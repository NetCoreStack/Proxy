using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreStack.Proxy
{
    public class ProxyUriDefinition
    {
        public static readonly IDictionary<string, string> TemplateCache =
            new Dictionary<string, string>();

        public UriBuilder UriBuilder { get; set; }

        public bool HasParameter { get; private set; }

        public List<string> TemplateKeys { get; private set; }

        public List<string> TemplateParameterKeys { get; private set; }

        public List<TemplatePart> ParameterParts { get; private set; }

        public ProxyUriDefinition(UriBuilder uriBuilder)
        {
            UriBuilder = uriBuilder;
            TemplateKeys = new List<string>();
            TemplateParameterKeys = new List<string>();
        }

        public void ResolveTemplate(RouteTemplate routeTemplate, string route, string template)
        {
            var path = UriBuilder.Path ?? string.Empty;
            if (routeTemplate != null)
            {
                if (routeTemplate.Parameters.Count > 0)
                {
                    HasParameter = true;
                    ParameterParts = new List<TemplatePart>(routeTemplate.Parameters);
                    var key = route + "/" + template;
                    if (TemplateCache.TryGetValue(key, out string tmpl))
                    {
                        path += $"{route}/{tmpl}";
                        UriBuilder.Path = path;
                        return;
                    }

                    TemplateKeys = routeTemplate.Segments
                        .SelectMany(s => s.Parts.Where(p => p.IsLiteral)
                        .Select(t => t.Text)).ToList();

                    TemplateParameterKeys = routeTemplate.Segments
                        .SelectMany(s => s.Parts.Where(p => p.IsParameter)
                        .Select(t => t.Name)).ToList();

                    tmpl = string.Join("/", TemplateKeys);
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
