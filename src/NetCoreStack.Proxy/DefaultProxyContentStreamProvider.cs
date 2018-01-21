using NetCoreStack.Proxy.Extensions;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public class DefaultProxyContentStreamProvider : IProxyContentStreamProvider
    {
        public ProxyMetadataProvider MetadataProvider { get; }
        public IModelContentResolver ContentResolver { get; }

        public DefaultProxyContentStreamProvider(ProxyMetadataProvider metadataProvider, IModelContentResolver contentResolver)
        {
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
            var isMultiPartFormData = descriptor.IsMultiPartFormData;
            var contentModelBinder = ContentBinderFactory.GetContentModelBinder(httpMethod);

            var bindingContext = new ContentModelBindingContext
            {
                HttpMethod = httpMethod,
                UriDefinition = proxyUriDefinition,
                MethodMarkerTemplate = descriptor.MethodMarkerTemplate,
                IsMultiPartFormData = isMultiPartFormData,
                ModelContentResolver = ContentResolver,
                Args = requestContext.Args,
                Parameters = descriptor.Parameters
            };

            contentModelBinder.BindContent(bindingContext);
            bindingContext.TrySetContent(request);
            request.RequestUri = bindingContext.Uri;
        }
    }
}
