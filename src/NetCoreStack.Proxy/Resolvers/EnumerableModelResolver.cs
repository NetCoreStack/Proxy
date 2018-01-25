namespace NetCoreStack.Proxy
{
    public class EnumerableModelResolver : ModelResolverBase
    {
        public override ModelResolverResult Resolve(ModelDictionaryContext context, ModelDictionaryResult result)
        {
            return ModelResolverResult.Failed();
        }
    }
}
