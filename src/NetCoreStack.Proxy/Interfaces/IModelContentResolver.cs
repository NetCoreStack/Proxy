using System.Collections.Generic;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public interface IModelContentResolver
    {
        ResolvedContentResult Resolve(HttpMethod httpMethod, 
            List<ProxyModelMetadata> parameters,
            bool isMultiPartFormData,
            object[] args);
    }
}
