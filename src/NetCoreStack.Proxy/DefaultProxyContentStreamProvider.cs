using NetCoreStack.Proxy.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public class DefaultProxyContentStreamProvider : IProxyContentStreamProvider
    {
        public async Task CreateRequestContentAsync(RequestContext requestContext, 
            HttpRequestMessage request, 
            ProxyMethodDescriptor descriptor, 
            UriBuilder uriBuilder)
        {
            var argsDic = descriptor.Resolve(requestContext.Args);
            if (descriptor.HttpMethod == HttpMethod.Post)
            {
                // TODO Streaming
                await Task.Run(() =>
                 {
                     if (argsDic.Count == 1)
                         request.Content = new StringContent(JsonConvert.SerializeObject(argsDic.First().Value), Encoding.UTF8, "application/json");
                     else
                         request.Content = new StringContent(JsonConvert.SerializeObject(argsDic), Encoding.UTF8, "application/json");
                 });
            }

            if (descriptor.HttpMethod == HttpMethod.Get)
            {
                uriBuilder = new UriBuilder(uriBuilder.Uri.ToQueryString(argsDic));
                request.RequestUri = uriBuilder.Uri;
            }
        }
    }
}
