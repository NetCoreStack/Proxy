namespace NetCoreStack.Proxy
{
    public interface IProxyContextFilter
    {
        void Invoke(ProxyContext proxyContext);
    }
}