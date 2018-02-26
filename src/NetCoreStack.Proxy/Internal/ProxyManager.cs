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
            _typeManager = typeManager ?? throw new ArgumentNullException(nameof(typeManager));
            _headerProvider = headerProvider ?? throw new ArgumentNullException(nameof(headerProvider));
            _httpClientAccessor = httpClientAccessor ?? throw new ArgumentNullException(nameof(httpClientAccessor));
            _endpointManager = endpointManager ?? throw new ArgumentNullException(nameof(endpointManager));
            _streamProvider = streamProvider ?? throw new ArgumentNullException(nameof(endpointManager));
            _options = options ?? throw new ArgumentNullException(nameof(options));

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

        private HttpRequestMessage CreateHttpRequest(ProxyMethodDescriptor methodDescriptor, RequestDescriptor requestDescriptor)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            var headers = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(requestDescriptor.ClientIp))
            {
                headers.Add("X-Forwarded-For", requestDescriptor.ClientIp);
            }

            if (!string.IsNullOrEmpty(requestDescriptor.UserAgent))
            {
                headers.Add("User-Agent", requestDescriptor.UserAgent);
            }

            if (requestDescriptor.Culture != null)
            {
                headers.Add("Accept-Language", requestDescriptor.Culture.Name);
            }

            headers.Merge(_headerProvider.Headers, true);

            headers.Merge(methodDescriptor.Headers, true);

            foreach (KeyValuePair<string, string> entry in headers)
            {
                requestMessage.Headers.Add(entry.Key, entry.Value);
            }

            return requestMessage;
        }

        public async Task<RequestContext> CreateRequestAsync(RequestDescriptor requestDescriptor)
        {
            var proxyDescriptor = _typeManager.ProxyDescriptors.FirstOrDefault(x => x.ProxyType == requestDescriptor.ProxyType);

            if (proxyDescriptor == null)
                throw new ArgumentOutOfRangeException("Proxy type could not be found!");

            var regionKey = proxyDescriptor.RegionKey;

            ProxyMethodDescriptor methodDescriptor;
            if (!proxyDescriptor.Methods.TryGetValue(requestDescriptor.TargetMethod, out methodDescriptor))
                throw new ArgumentOutOfRangeException("Method (Action) info could not be found!");

            HttpRequestMessage request = CreateHttpRequest(methodDescriptor, requestDescriptor);
            request.Method = methodDescriptor.HttpMethod;
            var methodPath = requestDescriptor.TargetMethod.Name;
            if (methodDescriptor.MethodMarkerTemplate.HasValue())
                methodPath = methodDescriptor.MethodMarkerTemplate;

            UriBuilder uriBuilder = _endpointManager.CreateUriBuilder(methodDescriptor, proxyDescriptor.Route, regionKey, methodPath);
            TimeSpan? timeout = methodDescriptor.Timeout;
            
            await _streamProvider.CreateRequestContentAsync(requestDescriptor, request, methodDescriptor, uriBuilder);

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
