namespace NetCoreStack.Proxy
{
    public interface IProxyEndpointManager
    {
        ProxyUriDefinition CreateUriDefinition(ProxyDescriptor descriptor, string regionKey, string targetMethodName);
    }
}
