using Microsoft.Extensions.Options;
using NetCoreStack.Proxy.Extensions;
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
        private readonly IHttpClientAccessor _httpClientAccessor;
        private readonly IOptions<ProxyOptions> _options;
        private readonly IDefaultHeaderProvider _headerProvider;
        private readonly IProxyEndpointManager _endpointManager;
        private readonly IProxyContentStreamProvider _streamProvider;

        public ProxyManager(IProxyTypeManager typeManager,
            IDefaultHeaderProvider headerProvider,
            IHttpClientAccessor httpClientAccessor,
            IProxyEndpointManager endpointManager,
            IProxyContentStreamProvider streamProvider,
            IOptions<ProxyOptions> options,
            IEnumerable<IProxyRequestFilter> requestFilters)
        {
            if (typeManager == null)
            {
                throw new ArgumentNullException(nameof(typeManager));
            }

            if (headerProvider == null)
            {
                throw new ArgumentNullException(nameof(headerProvider));
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
            _httpClientAccessor = httpClientAccessor;
            _endpointManager = endpointManager;
            _streamProvider = streamProvider;
            _options = options;

            RequestFilters = new List<IProxyRequestFilter>();
            if (requestFilters != null && requestFilters.Any())
            {
                HasFilter = true;
                RequestFilters = requestFilters.ToList();
            }
        }

        public HttpClient HttpClient
        {
            get
            {
                return _httpClientAccessor.HttpClient;
            }
        }

        public bool HasFilter { get; }
        public List<IProxyRequestFilter> RequestFilters { get; }

        private HttpRequestMessage CreateHttpRequest(ProxyMethodDescriptor methodDescriptor)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            foreach (KeyValuePair<string, string> entry in _headerProvider.Headers)
            {
                requestMessage.Headers.Add(entry.Key, entry.Value);
            }

            foreach (KeyValuePair<string, string> entry in methodDescriptor.Headers)
            {
                requestMessage.Headers.Add(entry.Key, entry.Value);
            }

            return requestMessage;
        }

        public async Task<RequestContext> CreateRequestAsync(RequestDescriptor descriptor)
        {
            var proxyDescriptor = _typeManager.ProxyDescriptors.FirstOrDefault(x => x.ProxyType == descriptor.ProxyType);

            if (proxyDescriptor == null)
                throw new ArgumentOutOfRangeException("Proxy type could not be found!");

            var regionKey = proxyDescriptor.RegionKey;

            ProxyMethodDescriptor methodDescriptor;
            if (!proxyDescriptor.Methods.TryGetValue(descriptor.TargetMethod, out methodDescriptor))
                throw new ArgumentOutOfRangeException("Method (Action) info could not be found!");

            HttpRequestMessage request = CreateHttpRequest(methodDescriptor);
            request.Method = methodDescriptor.HttpMethod;
            var methodPath = descriptor.TargetMethod.Name;
            if (methodDescriptor.MethodMarkerTemplate.HasValue())
                methodPath = methodDescriptor.MethodMarkerTemplate;

            ProxyUriDefinition proxyUriDefinition = _endpointManager.CreateUriDefinition(proxyDescriptor, regionKey, methodPath);
            TimeSpan? timeout = methodDescriptor.Timeout;

            request.RequestUri = proxyUriDefinition.UriBuilder.Uri;
            await _streamProvider.CreateRequestContentAsync(descriptor, request, methodDescriptor, proxyUriDefinition);

            return new RequestContext(request,
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
