using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class HeaderValues
    {
        public IDictionary<string, string> Headers { get; set; }

        public HeaderValues()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
