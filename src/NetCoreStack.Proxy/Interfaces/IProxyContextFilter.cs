using NetCoreStack.Proxy.Internal;

namespace NetCoreStack.Proxy
{
    public interface IProxyContextFilter
    {
        void Invoke(ProxyContext proxyContext);
    }
}