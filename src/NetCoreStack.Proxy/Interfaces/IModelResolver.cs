namespace NetCoreStack.Proxy
{
    public interface IModelResolver
    {
        ModelResolverResult Resolve(ModelDictionaryContext context, ModelDictionaryResult result);
    }
}