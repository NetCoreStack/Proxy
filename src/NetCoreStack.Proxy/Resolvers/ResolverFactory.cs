using System;

namespace NetCoreStack.Proxy
{
    public static class ResolverFactory
    {
        public static IModelResolver GetModelResolver(ProxyModelMetadata modelMetadata)
        {
            if (modelMetadata == null)
            {
                throw new ArgumentNullException(nameof(modelMetadata));
            }

            if (modelMetadata.IsEnumerableType)
            {
                return new EnumerableModelResolver();
            }

            if (modelMetadata.IsFormFile)
            {
                return new FormFileModelResolver();
            }

            if (modelMetadata.IsComplexType && !modelMetadata.IsCollectionType)
            {
                return new ComplexModelResolver();
            }

            return new SimpleModelResolver();
        }
    }
}