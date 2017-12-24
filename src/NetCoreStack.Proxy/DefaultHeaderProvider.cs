using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class DefaultHeaderProvider : IDefaultHeaderProvider
    {
        public IDictionary<string, string> Headers { get; set; }

        public DefaultHeaderProvider(IOptions<DefaultHeaderValues> options)
        {
            Headers = options.Value.Headers;
        }
    }
}
