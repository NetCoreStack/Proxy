using NetCoreStack.Proxy.Internal;
using System;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class ResponseContext
    {
        public RequestDescriptor RequestDescriptor { get; }
        public string Metadata { get; set; }
        public string MetadataInner { get; set; }
        public string ResultContent { get; set; }
        public HttpResponseMessage Response { get; set; }
        public object Value { get; set; }

        public ResponseContext(HttpResponseMessage response,
            RequestDescriptor requestDescriptor,
            string metadata, 
            string metadataInner, 
            Type genericReturnType = null)
        {

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (requestDescriptor == null)
            {
                throw new ArgumentNullException(nameof(requestDescriptor));
            }

            Response = response;
            RequestDescriptor = requestDescriptor;
            Metadata = metadata;
            MetadataInner = metadataInner;
        }
    }
}
