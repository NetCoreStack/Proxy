namespace NetCoreStack.Proxy
{
    public abstract class ModelResolverBase : IModelResolver
    {
        public abstract ModelResolverResult Resolve(ModelDictionaryContext context, ModelDictionaryResult result);
    }
}