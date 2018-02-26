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
            IConfiguration configuration,
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
            services.AddSingleton<ProxyMetadataProvider>();
            services.AddSingleton<IProxyTypeManager, DefaultProxyTypeManager>();

            services.TryAdd(ServiceDescriptor.Scoped<IProxyContextFilter, DefaultProxyContextFilter>());

            services.TryAdd(ServiceDescriptor.Singleton<IModelContentResolver, DefaultModelContentResolver>());
            services.TryAdd(ServiceDescriptor.Singleton<IHttpClientAccessor, DefaultHttpClientAccessor>());
            services.TryAdd(ServiceDescriptor.Singleton<IProxyManager, ProxyManager>());
            services.TryAdd(ServiceDescriptor.Singleton<IDefaultHeaderProvider, DefaultHeaderProvider>());
            services.TryAdd(ServiceDescriptor.Singleton<IProxyContentStreamProvider, DefaultProxyContentStreamProvider>());
            services.TryAdd(ServiceDescriptor.Singleton<IProxyEndpointManager, DefaultProxyEndpointManager>());
            services.TryAdd(ServiceDescriptor.Singleton<IModelJsonSerializer, DefaultModelJsonSerializer>());
            services.TryAdd(ServiceDescriptor.Singleton<IModelXmlSerializer, DefaultModelXmlSerializer>());

            services.TryAddSingleton<RoundRobinManager>();

            var options = new ProxyBuilderOptions();
            options.ModelResolvers.Add(new SimpleModelResolver());
            options.ModelResolvers.Add(new EnumerableModelResolver());
            options.ModelResolvers.Add(new ComplexModelResolver());
            options.ModelResolvers.Add(new SystemObjectModelResolver());
            options.ModelResolvers.Add(new FormFileModelResolver());
            setup?.Invoke(options);
            foreach (var item in options.ProxyList)
            {
                var type = item.GetTypeInfo().AsType();
                var genericRegistry = registryDelegate.MakeGenericMethod(type);
                genericRegistry.Invoke(null, new object[] { services });
            }

            if (options.ProxyContextFilter != null)
            {
                services.TryAdd(ServiceDescriptor.Scoped(typeof(IProxyContextFilter), options.ProxyContextFilter));
            }

            var headerValues = new DefaultHeaderValues { Headers = options.DefaultHeaders };
            services.AddSingleton(Options.Create(headerValues));
            services.AddSingleton(options);

            if (options.CultureFactory != null)
            {
                var defaultCultureFactory = new DefaultCultureFactory(options.CultureFactory);
                services.AddSingleton<ICultureFactory>(defaultCultureFactory);
            }
        }

        internal static void RegisterProxy<TProxy>(IServiceCollection services) where TProxy : IApiContract
        {
            services.AddScoped(typeof(TProxy), x => ProxyHelper.CreateProxy<TProxy>(x));
        }
    }
}
