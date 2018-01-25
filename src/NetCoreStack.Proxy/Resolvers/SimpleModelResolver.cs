namespace NetCoreStack.Proxy
{
    public class SimpleModelResolver : ModelResolverBase
    {
        public override ModelResolverResult Resolve(ModelDictionaryContext context, ModelDictionaryResult result)
        {
            return ModelResolverResult.Failed();
        }
    }
}
