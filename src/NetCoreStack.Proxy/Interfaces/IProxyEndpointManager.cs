namespace NetCoreStack.Proxy
{
    public interface IProxyEndpointManager
    {
        ProxyUriDefinition CreateUriDefinition(ProxyMethodDescriptor methodDescriptor, string route, string regionKey, string targetMethodName);
    }
}
