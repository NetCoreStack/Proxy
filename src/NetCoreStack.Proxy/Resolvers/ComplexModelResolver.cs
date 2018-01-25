namespace NetCoreStack.Proxy
{
    public class ComplexModelResolver : ModelResolverBase
    {
        public override ModelResolverResult Resolve(ModelDictionaryContext context, ModelDictionaryResult result)
        {
            return ModelResolverResult.Failed();
        }
    }
}
