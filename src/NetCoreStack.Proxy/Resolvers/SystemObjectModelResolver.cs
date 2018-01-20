using System;

namespace NetCoreStack.Proxy
{
    public class SystemObjectModelResolver : ModelResolverBase
    {
        public override ModelResolverResult Resolve(ModelDictionaryContext context, ModelDictionaryResult result)
        {
            return ModelResolverResult.Failed();
        }
    }
}