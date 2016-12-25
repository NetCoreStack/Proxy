using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public interface IHeaderProvider
    {
        IDictionary<string, string> Headers { get; set; }
    }
}
