using System;
using System.Diagnostics;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class ResponseContext: IDisposable
    {
        public RequestContext RequestContext { get; }
        public string ResultContent { get; set; }
        public HttpResponseMessage Response { get; set; }
        public object Value { get; set; }

        public ResponseContext(HttpResponseMessage response,
            RequestContext requestContext,
            Type genericReturnType = null)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
            RequestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
        }

        public void Dispose()
        {
            Debug.WriteLine("===Response Context disposing");
            RequestContext.Dispose();
            Response.Dispose();
        }
    }
}
