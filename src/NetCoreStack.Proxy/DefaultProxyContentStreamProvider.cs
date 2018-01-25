using NetCoreStack.Proxy.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public class DefaultProxyContentStreamProvider : IProxyContentStreamProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public ProxyMetadataProvider MetadataProvider { get; }
        public IModelContentResolver ContentResolver { get; }

        public DefaultProxyContentStreamProvider(IServiceProvider serviceProvider,
            ProxyMetadataProvider metadataProvider,
            IModelContentResolver contentResolver)
        {
            _serviceProvider = serviceProvider;
            MetadataProvider = metadataProvider;
            ContentResolver = contentResolver;
        }

        public async Task CreateRequestContentAsync(RequestDescriptor requestContext, 
            HttpRequestMessage request, 
            ProxyMethodDescriptor descriptor,
            ProxyUriDefinition proxyUriDefinition)
        {
            await Task.CompletedTask;

            var httpMethod = descriptor.HttpMethod;
            var contentModelBinder = ContentBinderFactory.GetContentModelBinder(_serviceProvider, httpMethod, descriptor.ContentType);

            var bindingContext = new ContentModelBindingContext(httpMethod, descriptor, proxyUriDefinition)
            {
                ContentType = descriptor.ContentType,
                ModelContentResolver = ContentResolver,
                Args = requestContext.Args,
            };

            contentModelBinder.BindContent(bindingContext);
            bindingContext.TrySetContent(request);
            request.RequestUri = bindingContext.Uri;
        }
    }
}
