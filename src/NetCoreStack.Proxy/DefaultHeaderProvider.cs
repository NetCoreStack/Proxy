using System;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class DefaultHeaderProvider : IHeaderProvider
    {
        public IDictionary<string, string> Headers { get; set; }

        public DefaultHeaderProvider()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
