using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class DefaultHttpClientAccessor : IHttpClientAccessor
    {
        public HttpClient HttpClient { get; set; }

        public DefaultHttpClientAccessor()
        {
            HttpClient = new HttpClient();
        }
    }
}
