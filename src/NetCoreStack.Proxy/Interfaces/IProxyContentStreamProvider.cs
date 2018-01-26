using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public interface IProxyContentStreamProvider
    {
        Task CreateRequestContentAsync(RequestDescriptor requestDescriptor, 
            HttpRequestMessage request, 
            ProxyMethodDescriptor descriptor, 
            UriBuilder uriBuilder);
    }
}