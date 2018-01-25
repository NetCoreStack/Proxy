namespace NetCoreStack.Proxy
{
    public class ModelDictionaryContext
    {
        public object Value { get; }
        public ProxyModelMetadata ModelMetadata { get; }

        public ModelDictionaryContext(ProxyModelMetadata modelMetadata, object value)
        {
            Value = value;
            ModelMetadata = modelMetadata;
        }
    }
}