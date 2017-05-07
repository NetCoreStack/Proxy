using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NetCoreStack.Proxy.Extensions;
using NetCoreStack.Proxy.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    internal class ProxyManager : IProxyManager
    {
        private readonly IProxyTypeManager _typeManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientAccessor _httpClientAccessor;
        private readonly IOptions<ProxyOptions> _options;
        private readonly IHeaderProvider _headerProvider;
        private readonly IProxyEndpointManager _endpointManager;
        private readonly IProxyContentStreamProvider _streamProvider;

        public ProxyManager(IProxyTypeManager typeManager,
            IHeaderProvider headerProvider,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientAccessor httpClientAccessor,
            IProxyEndpointManager endpointManager,
            IProxyContentStreamProvider streamProvider,
            IOptions<ProxyOptions> options)
        {
            if (typeManager == null)
            {
                throw new ArgumentNullException(nameof(typeManager));
            }

            if (headerProvider == null)
            {
                throw new ArgumentNullException(nameof(headerProvider));
            }

            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            if (httpClientAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpClientAccessor));
            }

            if (endpointManager == null)
            {
                throw new ArgumentNullException(nameof(endpointManager));
            }

            if (streamProvider == null)
            {
                throw new ArgumentNullException(nameof(endpointManager));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _typeManager = typeManager;
            _headerProvider = headerProvider;
            _httpContextAccessor = httpContextAccessor;
            _httpClientAccessor = httpClientAccessor;
            _endpointManager = endpointManager;
            _streamProvider = streamProvider;
            _options = options;
        }

        public HttpContext HttpContext
        {
            get
            {
                return _httpContextAccessor.HttpContext;
            }
        }

        public HttpClient HttpClient
        {
            get
            {
                return _httpClientAccessor.HttpClient;
            }
        }

        private HttpRequestMessage CreateHttpRequest(RequestContext context)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            foreach (KeyValuePair<string, string> entry in _headerProvider.Headers)
            {
                requestMessage.Headers.Add(entry.Key, entry.Value);
            } 

            return requestMessage;
        }

        public async Task<RequestDescriptor> CreateDescriptorAsync(RequestContext context)
        {
            HttpRequestMessage request = CreateHttpRequest(context);
            var proxyDescriptor = _typeManager.ProxyDescriptors.FirstOrDefault(x => x.ProxyType == context.ProxyType);

            if (proxyDescriptor == null)
                throw new ArgumentOutOfRangeException("Proxy type could not be found!");

            var regionKey = proxyDescriptor.RegionKey;

            ProxyMethodDescriptor methodDescriptor;
            if (!proxyDescriptor.Methods.TryGetValue(context.TargetMethod, out methodDescriptor))
                throw new ArgumentOutOfRangeException("Method (Action) info could not be found!");

            request.Method = methodDescriptor.HttpMethod;
            var methodPath = context.TargetMethod.Name;
            if (methodDescriptor.Template.HasValue())
                methodPath = methodDescriptor.Template;

            ProxyUriDefinition proxyUriDefinition = _endpointManager.CreateUriDefinition(proxyDescriptor, regionKey, methodPath);
            TimeSpan? timeout = methodDescriptor.Timeout;

            request.RequestUri = proxyUriDefinition.UriBuilder.Uri;
            await _streamProvider.CreateRequestContentAsync(context, request, methodDescriptor, proxyUriDefinition);

            return new RequestDescriptor(request,
                methodDescriptor,
                proxyDescriptor.RegionKey,
                timeout);
        }

        public HttpClient GetHttpClient()
        {
            return _httpClientAccessor.HttpClient;
        }
    }
}
