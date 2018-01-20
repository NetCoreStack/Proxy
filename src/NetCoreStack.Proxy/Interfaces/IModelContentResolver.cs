using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public interface IModelContentResolver
    {
        ModelDictionaryResult Resolve(List<ProxyModelMetadata> parameters, object[] args);
    }
}
