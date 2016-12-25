using NetCoreStack.Proxy.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class HttpResponseExtensions
    {
        public static string GetMetadataHeader(this HttpResponseMessage response)
        {
            IEnumerable<string> metadaHeader;
            if (response.Headers.TryGetValues(Constants.MetadataHeader, out metadaHeader))
            {
                return metadaHeader.FirstOrDefault();
            }
            return string.Empty;
        }

        public static string GetMetadataInnerHeader(this HttpResponseMessage response)
        {
            IEnumerable<string> metadaHeader;
            if (response.Headers.TryGetValues(Constants.MetadataInnerHeader, out metadaHeader))
            {
                return metadaHeader.FirstOrDefault();
            }
            return string.Empty;
        }
    }
}
