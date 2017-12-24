using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public interface IProxyRequestFilter
    {
        Task InvokeAsync(RequestContext context);
    }
}