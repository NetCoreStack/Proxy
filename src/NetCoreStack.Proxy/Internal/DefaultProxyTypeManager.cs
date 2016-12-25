using NetCoreStack.Common;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using NetCoreStack.Proxy.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;
using NetCoreStack.Common.Extensions;

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
                var path = proxyType.GetTypeInfo().GetCustomAttribute<ApiRouteAttribute>();
                if (path == null)
                    throw new ArgumentNullException($"{nameof(ApiRouteAttribute)} required for Proxy - Api interface.");

                if (!path.RegionKey.HasValue())
                    throw new ArgumentOutOfRangeException($"Specify the \"{nameof(path.RegionKey)}\"!");

                var route = proxyType.Name.GetApiRawName(path.Template);

                ProxyDescriptor descriptor = new ProxyDescriptor(proxyType, path.RegionKey, route);

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
                        .OfType<HttpMethodAttribute>().FirstOrDefault();

                    if (httpMethodAttribute != null)
                    {
                        if (httpMethodAttribute is HttpPostAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Post;

                        else if (httpMethodAttribute is HttpGetAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Get;

                        else if (httpMethodAttribute is HttpPutAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Put;

                        else if (httpMethodAttribute is HttpDeleteAttribute)
                            proxyMethodDescriptor.HttpMethod = HttpMethod.Delete;
                    }
                    else
                    {
                        // Default GET
                        proxyMethodDescriptor.HttpMethod = HttpMethod.Get;
                    }

                    proxyMethodDescriptor.Parameters = new List<ParameterDescriptor>();
                    foreach (var parameter in method.GetParameters())
                    {
                        //if (proxyMethodDescriptor.HttpMethod == HttpMethod.Get)
                        //{
                        //    if (parameter.ParameterType.IsReferenceType() &&
                        //        !typeof(IQueryStringTransferable).IsAssignableFrom(parameter.ParameterType))
                        //    {
                        //        throw new InvalidOperationException($"API methods that take a reference type parameter and are marked with HttpGet " +
                        //            $"should be derived from the {nameof(IQueryStringTransferable)} " +
                        //           $"or method should be marked as HttpPost. MethodName: \"{method.Name}\", " + 
                        //           $"ParameterType: \"{parameter.ParameterType.Name}\"");
                        //    }
                        //}

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
