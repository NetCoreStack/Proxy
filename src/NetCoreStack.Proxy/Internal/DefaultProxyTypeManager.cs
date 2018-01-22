using Microsoft.AspNetCore.Routing.Template;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace NetCoreStack.Proxy.Internal
{
    public class DefaultProxyTypeManager : IProxyTypeManager
    {
        private readonly ProxyBuilderOptions _options;

        public IList<ProxyDescriptor> ProxyDescriptors { get; set; }
        public ProxyMetadataProvider MetadataProvider { get; }

        public DefaultProxyTypeManager(ProxyMetadataProvider metadataProvider, ProxyBuilderOptions options)
        {
            MetadataProvider = metadataProvider;
            _options = options;
            ProxyDescriptors = GetProxyDescriptors();
        }

        private IList<ProxyDescriptor> GetProxyDescriptors()
        {
            var baseMethods = typeof(IApiContract).GetMethods().Where(x => !x.IsSpecialName).ToList();
            var types = _options.ProxyList;
            var descriptors = new List<ProxyDescriptor>();
            foreach (var proxyType in types)
            {
                var pathAttr = proxyType.GetTypeInfo().GetCustomAttribute<ApiRouteAttribute>();
                if (pathAttr == null)
                    throw new ProxyException($"{nameof(ApiRouteAttribute)} required for Proxy - Api interface.");

                if (!pathAttr.RegionKey.HasValue())
                    throw new ProxyException($"Specify the \"{nameof(pathAttr.RegionKey)}\"!");

                var route = proxyType.Name.GetApiRootPath(pathAttr.RouteTemplate);

                ProxyDescriptor descriptor = new ProxyDescriptor(proxyType, pathAttr.RegionKey, route);

                var interfaces = proxyType.GetInterfaces()
                    .Except(new List<Type> { typeof(IApiContract) }).ToList();

                var interfaceMethods = new List<MethodInfo>();
                if (interfaces.Any())
                {
                    foreach (var type in interfaces)
                    {
                        interfaceMethods.AddRange(type.GetMethods().Where(x => !x.IsSpecialName).ToList());
                    }
                }

                var methods = proxyType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).ToList();
                interfaceMethods.AddRange(baseMethods);
                methods.AddRange(interfaceMethods);
                
                foreach (var method in methods)
                {
                    var proxyMethodDescriptor = new ProxyMethodDescriptor(method);

                    var timeoutAttr = method.GetCustomAttribute<ApiTimeoutAttribute>();
                    if (timeoutAttr != null)
                        proxyMethodDescriptor.Timeout = timeoutAttr.Timeout;

                    var httpMethodAttribute = method.GetCustomAttributes(inherit: true)
                        .OfType<HttpMethodMarkerAttribute>().FirstOrDefault();

                    var httpHeaders = method.GetCustomAttribute<HttpHeadersAttribute>();
                    if (httpHeaders != null && httpHeaders.Headers != null)
                    {
                        foreach (var header in httpHeaders.Headers)
                        {
                            var token = header.Split(':');
                            if (token.Length > 1)
                            {
                                proxyMethodDescriptor.Headers[token[0].Trim()] = token[1].Trim();
                            }
                        }
                    }

                    if (httpMethodAttribute != null)
                    {
                        if (httpMethodAttribute.Template.HasValue())
                        {
                            var template = httpMethodAttribute.Template;
                            proxyMethodDescriptor.MethodMarkerTemplate = template;
                            var routeTemplate = TemplateParser.Parse(template);
                            if (routeTemplate != null)
                            {
                                proxyMethodDescriptor.RouteTemplate = routeTemplate;
                                proxyMethodDescriptor.ParameterParts = new List<TemplatePart>(routeTemplate.Parameters);

                                proxyMethodDescriptor.TemplateKeys = routeTemplate.Segments
                                    .SelectMany(s => s.Parts.Where(p => p.IsLiteral)
                                    .Select(t => t.Text)).ToList();

                                proxyMethodDescriptor.TemplateParameterKeys = routeTemplate.Segments
                                    .SelectMany(s => s.Parts.Where(p => p.IsParameter)
                                    .Select(t => t.Name)).ToList();
                            }
                        }

                        if (httpMethodAttribute is HttpGetMarkerAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Get;
                        else if (httpMethodAttribute is HttpPostMarkerAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Post;
                        else if (httpMethodAttribute is HttpPutMarkerAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Put;
                        else if (httpMethodAttribute is HttpDeleteMarkerAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Delete;
                    }
                    else
                    {
                        // Default GET
                        proxyMethodDescriptor.HttpMethod = HttpMethod.Get;
                    }

                    bool isMultipartFormData = false;
                    foreach (var parameter in method.GetParameters())
                    {
                        var modelMetadata = MetadataProvider.GetMetadataForParameter(parameter);
                        if (modelMetadata.IsFormFile)
                        {
                            isMultipartFormData = true;
                            if (parameter.CustomAttributes
                                .Any(a => a.AttributeType.Name == "FromBodyAttribute"))
                            {
                                throw new ProxyException($"Parameter: \"{parameter.ParameterType.Name} as {parameter.Name}\" " +
                                    "contains IFormFile type property. " +
                                    "Remove FromBody attribute to proper model binding.");
                            }
                        }

                        proxyMethodDescriptor.Parameters.Add(modelMetadata);
                    }
                    
                    proxyMethodDescriptor.IsMultiPartFormData = isMultipartFormData;
                    descriptor.Methods.Add(method, proxyMethodDescriptor);
                }

                descriptors.Add(descriptor);
            }

            return descriptors;
        }
    }
}
