using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public interface IProxyTypeManager
    {
        IList<ProxyDescriptor> ProxyDescriptors { get; set; }
    }
}
