using System;

namespace NetCoreStack.Proxy
{
    public interface IProxyEndpointManager
    {
        UriBuilder CreateUriBuilder(ProxyDescriptor descriptor, string regionKey, string targetMethodName);
    }
}
