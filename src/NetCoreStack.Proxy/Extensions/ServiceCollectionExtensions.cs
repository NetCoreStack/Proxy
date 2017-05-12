using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Internal;
using System;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public static class ServiceCollectionExtensions
    {
        private static readonly MethodInfo registryDelegate = 
            typeof(ServiceCollectionExtensions).GetTypeInfo().GetDeclaredMethod("RegisterProxy");

        public static void AddNetCoreProxy(this IServiceCollection services, 
            IConfigurationRoot configuration,
            Action<ProxyBuilderOptions> setup)
        {
            services.AddOptions();

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (setup == null)
            {
                throw new ArgumentNullException(nameof(setup));
            }

            services.Configure<ProxyOptions>(configuration.GetSection(Constants.ProxySettings));
            services.AddSingleton<IProxyTypeManager, DefaultProxyTypeManager>();

            services.TryAdd(ServiceDescriptor.Singleton<IHttpContextAccessor, HttpContextAccessor>());
            services.TryAdd(ServiceDescriptor.Singleton<IHttpClientAccessor, DefaultHttpClientAccessor>());
            services.TryAdd(ServiceDescriptor.Singleton<IProxyManager, ProxyManager>());
            services.TryAdd(ServiceDescriptor.Singleton<IHeaderProvider, DefaultHeaderProvider>());
            services.TryAdd(ServiceDescriptor.Singleton<IProxyContentStreamProvider, DefaultProxyContentStreamProvider>());
            services.TryAdd(ServiceDescriptor.Singleton<IProxyEndpointManager, DefaultProxyEndpointManager>());

            services.TryAddSingleton<RoundRobinManager>();

            var proxyBuilderOptions = new ProxyBuilderOptions();
            setup?.Invoke(proxyBuilderOptions);
            foreach (var item in proxyBuilderOptions.ProxyList)
            {
                var type = item.GetTypeInfo().AsType();
                var genericRegistry = registryDelegate.MakeGenericMethod(type);
                genericRegistry.Invoke(null, new object[] { services });
            }

            var headerValues = new HeaderValues { Headers = proxyBuilderOptions.DefaultHeaders };
            services.AddSingleton(Options.Create(headerValues));
            services.AddSingleton(proxyBuilderOptions);
        }

        internal static void RegisterProxy<TProxy>(IServiceCollection services) where TProxy : IApiContract
        {
            services.AddScoped(typeof(TProxy), x => ProxyHelper.CreateProxy<TProxy>(x));
        }
    }
}
