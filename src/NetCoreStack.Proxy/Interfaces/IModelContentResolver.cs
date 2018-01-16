using System.Collections.Generic;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public interface IModelContentResolver
    {
        ResolvedContentResult Resolve(List<ProxyModelMetadata> parameters,
            HttpMethod httpMethod,
            bool isMultiPartFormData,
            object[] args);
    }
}
