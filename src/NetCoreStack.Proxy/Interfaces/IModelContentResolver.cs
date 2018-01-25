using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public interface IModelContentResolver
    {
        ModelDictionaryResult Resolve(List<ProxyModelMetadata> parameters, object[] args, int parameterOffset = 0, bool ignoreModelPrefix = false);

        string ResolveParameter(ProxyModelMetadata parameter, object value, bool isTopLevelObject, string prefix = "");
    }
}