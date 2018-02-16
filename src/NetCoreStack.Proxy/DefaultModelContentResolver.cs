using Microsoft.AspNetCore.Http;
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

        private void TrySetFormFile(string prefix, ProxyModelMetadata modelMetadata, ModelDictionaryResult result, object value)
        {
            if (modelMetadata.IsNullableValueType)
            {
                // recall with prefix
                TrySetFormFile(prefix, modelMetadata.ElementType, result, value);
                return;
            }

            if (modelMetadata.IsEnumerableType)
            {
                var enumerable = value as IEnumerable<IFormFile>;
                int index = 0;
                foreach (var item in enumerable)
                {
                    if (item != null)
                    {
                        result.Files.Add($"{prefix}[{index}]", item);
                    }
                    index++;
                }

                return;
            }

            if (modelMetadata.IsFormFile)
            {
                result.Files.Add(prefix, (IFormFile)value);
            }            
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
            ModelDictionaryResult result, 
            object value)
        {
            var objModelMetadata = new ProxyModelMetadata(propertyInfo,
                    ProxyModelMetadataIdentity.ForProperty(value.GetType(),
                    propertyName,
                    containerType));


            if (objModelMetadata.IsSimpleType)
            {
                result.Dictionary.Add(key, Convert.ToString(value));
                return;
            }

            if (objModelMetadata.IsEnumerableType)
            {
                if (objModelMetadata.ElementType.IsSimpleType)
                {
                    SetSimpleEnumerable(key, result.Dictionary, value);
                }
                else
                {
                    TrySetValueInner(key, objModelMetadata, result, value);
                }

                return;
            }

            // Anonymous object resolver
            for (int i = 0; i < objModelMetadata.Properties.Count; i++)
            {
                var modelMetadata = objModelMetadata.Properties[i];
                var v = modelMetadata.PropertyInfo.GetValue(value);
                if (v != null)
                {
                    ResolveInternal(modelMetadata, result, v, isTopLevelObject: false, prefix: key);
                }
            }            
        }

        private void TrySetValueInner(string key, ProxyModelMetadata modelMetadata, ModelDictionaryResult result, object value)
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
                        if (v == null)
                        {
                            continue;
                        }

                        var propKey = $"{key}[{index}]";
                        var propValue = propertyInfo?.GetValue(v);
                        ResolveInternal(elementModelMetadata, result, propValue, isTopLevelObject:false, prefix: propKey);
                        index++;
                    }
                }
            }
        }

        private void TrySetValue(string prefix, ProxyModelMetadata modelMetadata, ModelDictionaryResult result, object value)
        {
            var key = prefix;

            if (modelMetadata.IsNullableValueType)
            {
                // recall with prefix
                TrySetValue(prefix, modelMetadata.ElementType, result, value);
                return;
            }

            if (modelMetadata.IsSimpleType)
            {
                result.Dictionary.Add(key, Convert.ToString(value));
                return;
            }

            if (modelMetadata.IsEnumerableType)
            {
                if (modelMetadata.ElementType.IsSimpleType)
                {
                    SetSimpleEnumerable(key, result.Dictionary, value);
                }
                else if(modelMetadata.ElementType.IsSystemObject)
                {
                    TrySetSystemObjectValue(key, 
                        modelMetadata.ElementType.PropertyInfo,
                        modelMetadata.ElementType.PropertyName, 
                        modelMetadata.ContainerType, 
                        result, value);
                }
                else
                {
                    TrySetValueInner(key, modelMetadata, result, value);
                }

                return;
            }
            
            if (modelMetadata.IsSystemObject)
            {
                TrySetSystemObjectValue(key, modelMetadata.PropertyInfo, modelMetadata.PropertyName, modelMetadata.ContainerType, result, value);
            }
        }

        private void ResolveInternal(ProxyModelMetadata modelMetadata, ModelDictionaryResult result, object value, bool isTopLevelObject = false, string prefix = "")
        {
            var key = isTopLevelObject ? string.Empty : modelMetadata.PropertyName;
            if (!string.IsNullOrEmpty(prefix))
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = prefix;
                }
                else
                {
                    key = $"{prefix}.{key}";
                }
            }

            if (modelMetadata.IsFormFile || (modelMetadata.IsEnumerableType && modelMetadata.ElementType.IsFormFile))
            {
                TrySetFormFile(key, modelMetadata, result, value);
                return;
            }

            var dictionary = result.Dictionary;
            var count = modelMetadata.PropertiesCount;
            if (count == 0)
            {
                TrySetValue(key, modelMetadata, result, value);
                return;
            }

            for (int i = 0; i < modelMetadata.PropertiesCount; i++)
            {
                var metadata = modelMetadata.Properties[i];
                if (metadata.ContainerType != null)
                {
                    if (value != null)
                    {
                        var v = metadata.PropertyInfo.GetValue(value);
                        if (v != null)
                        {
                            ResolveInternal(metadata, result, v, isTopLevelObject: false, prefix: key);
                        }
                    }
                }
                else
                {
                    ResolveInternal(metadata, result, value);
                }
            }
        }

        // Per request parameter context resolver
        public ModelDictionaryResult Resolve(List<ProxyModelMetadata> parameters, object[] args, int parameterOffset = 0, bool ignoreModelPrefix = false)
        {
            var result = new ModelDictionaryResult(new Dictionary<string, string>(StringComparer.Ordinal), 
                new Dictionary<string, IFormFile>(StringComparer.Ordinal));
            
            for (int i = parameterOffset; i < parameters.Count; i++)
            {
                var modelMetadata = parameters[i];
                var modelPrefix = string.Empty;
                if (!ignoreModelPrefix)
                {
                    if (modelMetadata.ContainerType == null && !string.IsNullOrEmpty(modelMetadata.PropertyName))
                    {
                        modelPrefix = modelMetadata.PropertyName;
                    }
                }
                ResolveInternal(modelMetadata, result, args[i], isTopLevelObject: true, prefix: modelPrefix);
            }

            return result;
        }

        public string ResolveParameter(ProxyModelMetadata modelMetadata, object value, bool isTopLevelObject, string prefix = "")
        {
            var result = new ModelDictionaryResult(new Dictionary<string, string>(StringComparer.Ordinal),
               new Dictionary<string, IFormFile>(StringComparer.Ordinal));

            ResolveInternal(modelMetadata, result, value, isTopLevelObject: isTopLevelObject, prefix: prefix);

            if(result.Dictionary.TryGetValue(modelMetadata.PropertyName, out string propValue))
            {
                return propValue;
            }
            else
            {
                throw new InvalidOperationException($"The parameter does not found in resolver dictionary! " +
                    $"Name: \"{modelMetadata.PropertyName}\", Type: \"{modelMetadata.ModelType.Name}\"");
            }
        }
    }
}