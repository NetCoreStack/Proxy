using Microsoft.AspNetCore.Http;
using NetCoreStack.Proxy.Internal;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public interface IProxyManager
    {
        HttpContext HttpContext { get; }
        HttpClient HttpClient { get; }
        Task<RequestDescriptor> CreateDescriptorAsync(RequestContext context);
    }
}
