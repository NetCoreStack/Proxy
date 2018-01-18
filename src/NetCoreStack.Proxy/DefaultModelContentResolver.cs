using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;

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

        private void TrySetSystemObjectValue()
        {

        }

        private void TrySetValueInner(string key, ProxyModelMetadata modelMetadata, Dictionary<string, string> dictionary, object value)
        {
            var count = modelMetadata.ElementType.Properties.Count;
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
                TrySetValue(prefix, modelMetadata.ElementType, dictionary, value);
                return;
            }

            if (modelMetadata.IsSimpleType)
            {
                dictionary.Add(key, Convert.ToString(value));
                return;
            }

            if (modelMetadata.IsEnumerableType)
            {
                if (modelMetadata.ElementType.IsSimpleType)
                {
                    SetSimpleEnumerable(key, dictionary, value);
                }
                else if(modelMetadata.ElementType.IsSystemObject)
                {

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
                // TODO
                // TrySetSystemObjectValue

                var objModelMetadata = new ProxyModelMetadata(modelMetadata.PropertyInfo,
                    ProxyModelMetadataIdentity.ForProperty(value.GetType(),
                    modelMetadata.PropertyName,
                    modelMetadata.ContainerType));

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
                }
            }
        }

        private void ResolveInternal(ProxyModelMetadata modelMetadata, Dictionary<string, string> dictionary, object value, string prefix = "")
        {
            var count = modelMetadata.Properties.Count;
            if (count == 0)
            {
                TrySetValue(prefix, modelMetadata, dictionary, value);
                return;
            }

            for (int i = 0; i < modelMetadata.Properties.Count; i++)
            {
                var metadata = modelMetadata.Properties[i];
                if (metadata.ContainerType != null)
                {
                    var parent = prefix;
                    if (!metadata.IsSimpleType && 
                        !metadata.IsEnumerableType &&
                        !metadata.IsNullableValueType)
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
        public ResolvedContentResult Resolve(HttpMethod httpMethod,
            List<ProxyModelMetadata> parameters,
            bool isMultiPartFormData,
            object[] args)
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
