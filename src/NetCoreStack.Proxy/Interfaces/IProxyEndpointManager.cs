using System;

namespace NetCoreStack.Proxy
{
    public interface IProxyEndpointManager
    {
        UriBuilder CreateUriBuilder(ProxyMethodDescriptor methodDescriptor, string route, string regionKey, string targetMethodName);
    }
}
