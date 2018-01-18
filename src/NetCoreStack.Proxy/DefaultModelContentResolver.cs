using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public class DefaultModelContentResolver : IModelContentResolver
    {
        public ProxyMetadataProvider MetadataProvider { get; }

        public DefaultModelContentResolver(ProxyMetadataProvider metadataProvider)
        {
            MetadataProvider = metadataProvider;
        }

        private void SetSimpleEnumerable(string key, Dictionary<string, string> dictionary, object value)
        {
            var enumerable = value as IEnumerable;
            int index = 0;
            foreach (var item in enumerable)
            {
                if (item != null)
                {
                    dictionary.Add($"{key}[{index}]", Convert.ToString(item));
                }
                index++;
            }
        }

        private void TrySetSystemObjectValue(string key, 
            PropertyInfo propertyInfo, 
            string propertyName, 
            Type containerType, 
            Dictionary<string, string> dictionary, 
            object value)
        {
            var objModelMetadata = new ProxyModelMetadata(propertyInfo,
                    ProxyModelMetadataIdentity.ForProperty(value.GetType(),
                    propertyName,
                    containerType));


            if (objModelMetadata.IsSimpleType)
            {
                dictionary.Add(key, Convert.ToString(value));
                return;
            }

            if (objModelMetadata.IsEnumerableType)
            {
                if (objModelMetadata.ElementType.IsSimpleType)
                {
                    SetSimpleEnumerable(key, dictionary, value);
                }
                else
                {
                    TrySetValueInner(key, objModelMetadata, dictionary, value);
                }

                return;
            }

            // Anonymous object resolver
            ResolveInternal(objModelMetadata, dictionary, value, key);
        }

        private void TrySetValueInner(string key, ProxyModelMetadata modelMetadata, Dictionary<string, string> dictionary, object value)
        {
            var count = modelMetadata.ElementType.PropertiesCount;
            var elementProperties = modelMetadata.ElementType.Properties;
            for (int i = 0; i < count; i++)
            {
                var elementModelMetadata = elementProperties[i];
                var propertyInfo = elementModelMetadata.PropertyInfo;
                if (value is IEnumerable values)
                {
                    var index = 0;
                    foreach (var v in values)
                    {
                        var propKey = $"{key}[{index}]";
                        var propValue = propertyInfo?.GetValue(v);
                        ResolveInternal(elementModelMetadata, dictionary, propValue, propKey);
                        index++;
                    }
                }
            }
        }

        private void TrySetValue(string prefix, ProxyModelMetadata modelMetadata, Dictionary<string, string> dictionary, object value)
        {
            var key = modelMetadata.PropertyName;
            if (!string.IsNullOrEmpty(prefix))
                key = $"{prefix}.{key}";

            if (modelMetadata.IsNullableValueType)
            {
                // recall with prefix
                TrySetValue(prefix, modelMetadata.ElementType, dictionary, value);
                return;
            }

            if (modelMetadata.IsSimpleType)
            {
                dictionary.Add(key, Convert.ToString(value));
                return;
            }

            if (modelMetadata.IsFormFile)
            {
                // TODO Gencebay
            }

            if (modelMetadata.IsEnumerableType)
            {
                if (modelMetadata.ElementType.IsSimpleType)
                {
                    SetSimpleEnumerable(key, dictionary, value);
                }
                else if(modelMetadata.ElementType.IsSystemObject)
                {
                    TrySetSystemObjectValue(key, 
                        modelMetadata.ElementType.PropertyInfo,
                        modelMetadata.ElementType.PropertyName, 
                        modelMetadata.ContainerType, 
                        dictionary, value);
                }
                else
                {
                    TrySetValueInner(key, modelMetadata, dictionary, value);
                }

                return;
            }

            // If typeof(object)
            if (modelMetadata.IsSystemObject)
            {
                TrySetSystemObjectValue(key, modelMetadata.PropertyInfo, modelMetadata.PropertyName, modelMetadata.ContainerType, dictionary, value);
            }
        }

        private void ResolveInternal(ProxyModelMetadata modelMetadata, Dictionary<string, string> dictionary, object value, string prefix = "")
        {
            var count = modelMetadata.PropertiesCount;
            if (count == 0)
            {
                TrySetValue(prefix, modelMetadata, dictionary, value);
                return;
            }

            for (int i = 0; i < modelMetadata.PropertiesCount; i++)
            {
                var metadata = modelMetadata.Properties[i];
                if (metadata.ContainerType != null)
                {
                    var parent = prefix;
                    if (!metadata.IsSimpleType && 
                        !metadata.IsEnumerableType &&
                        !metadata.IsNullableValueType &&
                        !metadata.IsSystemObject)
                    {
                        parent = !string.IsNullOrEmpty(prefix) ? $"{prefix}.{metadata.PropertyName}" : metadata.PropertyName;
                    }

                    if (value != null)
                    {
                        var v = metadata.PropertyInfo.GetValue(value);
                        if (v != null)
                        {
                            ResolveInternal(metadata, dictionary, v, parent);
                        }
                    }
                }
                else
                {
                    ResolveInternal(metadata, dictionary, value);
                }
            }
        }

        // Per request parameter context resolver
        public ResolvedContentResult Resolve(List<ProxyModelMetadata> parameters, object[] args)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
            var count = parameters.Count;

            for (int i = 0; i < count; i++)
            {
                var modelMetadata = parameters[i];
                ResolveInternal(modelMetadata, dictionary, args[i]);
            }

            return new ResolvedContentResult(dictionary);
        }
    }
}
