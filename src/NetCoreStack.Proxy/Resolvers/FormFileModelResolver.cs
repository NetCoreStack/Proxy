namespace NetCoreStack.Proxy
{
    public class FormFileModelResolver : ModelResolverBase
    {
        public override ModelResolverResult Resolve(ModelDictionaryContext context, ModelDictionaryResult result)
        {
            return ModelResolverResult.Failed();
        }
    }
}
