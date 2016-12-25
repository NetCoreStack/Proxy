using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public interface IHttpClientAccessor
    {
        HttpClient HttpClient { get; set; } 
    }
}
