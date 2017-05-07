using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class DefaultHeaderProvider : IHeaderProvider
    {
        public IDictionary<string, string> Headers { get; set; }

        public DefaultHeaderProvider(IOptions<HeaderValues> options)
        {
            Headers = options.Value.Headers;
        }
    }
}
