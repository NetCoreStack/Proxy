using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        public DefaultProxyTypeManager(ProxyBuilderOptions options)
        {
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
                    throw new ArgumentNullException($"{nameof(ApiRouteAttribute)} required for Proxy - Api interface.");

                if (!pathAttr.RegionKey.HasValue())
                    throw new ArgumentOutOfRangeException($"Specify the \"{nameof(pathAttr.RegionKey)}\"!");

                var route = proxyType.Name.GetApiRawName(pathAttr.RouteTemplate);

                ProxyDescriptor descriptor = new ProxyDescriptor(proxyType, pathAttr.RegionKey, route);

                var interfaces = proxyType.GetInterfaces()
                    .Except(new List<Type> { typeof(IApiContract), typeof(IDependency) }).ToList();

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

                #region method resolver
                foreach (var method in methods)
                {
                    var proxyMethodDescriptor = new ProxyMethodDescriptor(method);

                    var timeoutAttr = method.GetCustomAttribute<ApiTimeoutAttribute>();
                    if (timeoutAttr != null)
                        proxyMethodDescriptor.Timeout = timeoutAttr.Timeout;

                    var httpMethodAttribute = method.GetCustomAttributes(inherit: true)
                        .OfType<HttpMethodMarkerAttribute>().FirstOrDefault();

                    if (httpMethodAttribute != null)
                    {
                        if (httpMethodAttribute is HttpPostMarkerAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Post;
                    }
                    else
                    {
                        // Default GET
                        proxyMethodDescriptor.HttpMethod = HttpMethod.Get;
                    }

                    proxyMethodDescriptor.Parameters = new List<ParameterDescriptor>();
                    foreach (var parameter in method.GetParameters())
                    {
                        proxyMethodDescriptor.Parameters.Add(new ParameterDescriptor()
                        {
                            Name = parameter.Name,
                            ParameterType = parameter.ParameterType,
                            BindingInfo = BindingInfo.GetBindingInfo(parameter.GetCustomAttributes().OfType<object>())
                        });
                    }

                    descriptor.Methods.Add(method, proxyMethodDescriptor);
                }
                #endregion // method resolver

                descriptors.Add(descriptor);
            }

            return descriptors;
        }
    }
}
