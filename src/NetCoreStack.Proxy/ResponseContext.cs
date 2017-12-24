using System;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class ResponseContext
    {
        public RequestContext RequestDescriptor { get; }
        public string ResultContent { get; set; }
        public HttpResponseMessage Response { get; set; }
        public object Value { get; set; }

        public ResponseContext(HttpResponseMessage response,
            RequestContext requestDescriptor,
            Type genericReturnType = null)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
            RequestDescriptor = requestDescriptor ?? throw new ArgumentNullException(nameof(requestDescriptor));
        }
    }
}
