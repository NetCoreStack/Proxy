namespace NetCoreStack.Proxy
{
    public interface IProxyContextAccessor
    {
        ProxyContext ProxyContext { get; set; }
    }
}