using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public interface IProxyContentStreamProvider
    {
        Task CreateRequestContentAsync(RequestDescriptor requestContext, 
            HttpRequestMessage request, 
            ProxyMethodDescriptor descriptor, 
            ProxyUriDefinition proxyUriDefinition);
    }
}