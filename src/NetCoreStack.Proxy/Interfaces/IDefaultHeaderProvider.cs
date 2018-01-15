using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public interface IDefaultHeaderProvider
    {
        IDictionary<string, string> Headers { get; set; }
    }
}