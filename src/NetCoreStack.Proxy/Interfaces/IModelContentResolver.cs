using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public interface IModelContentResolver
    {
        ResolvedContentResult Resolve(List<ProxyModelMetadata> parameters, object[] args);
    }
}
