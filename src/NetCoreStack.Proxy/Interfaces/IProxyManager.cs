using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public interface IProxyManager
    {
        bool HasFilter { get; }
        HttpClient HttpClient { get; }
        List<IProxyRequestFilter> RequestFilters { get; }
        Task<RequestContext> CreateRequestAsync(RequestDescriptor context);
    }
}