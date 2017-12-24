using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class DefaultHeaderValues
    {
        public IDictionary<string, string> Headers { get; set; }

        public DefaultHeaderValues()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
