using NetCoreStack.Proxy.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        private void TrySetValue(string prefix, ProxyModelMetadata modelMetadata, Dictionary<string, string> dictionary, object value)
        {
            var key = modelMetadata.PropertyName;
            if (!string.IsNullOrEmpty(prefix))
                key = $"{prefix}.{key}";

            if (modelMetadata.IsSimpleType)
            {
                if (modelMetadata.PropertyInfo != null)
                {
                    dictionary.Add(key, Convert.ToString(value));
                    return;
                }
            }

            if (modelMetadata.IsEnumerableType)
            {
                if (modelMetadata.IsElementTypeSimple)
                {
                    SetSimpleEnumerable(key, dictionary, value);
                }
                else
                {
                    if (TypeDescriptor.GetConverter(value.GetType()).CanConvertTo(typeof(string)))
                    {
                        SetSimpleEnumerable(key, dictionary, value);
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
                    if (!metadata.IsSimpleType && !metadata.IsEnumerableType)
                    {
                        if (!string.IsNullOrEmpty(prefix))
                        {
                            parent = $"{prefix}.{metadata.PropertyName}";
                        }
                        else
                        {
                            parent = metadata.PropertyName;
                        }
                    }

                    var v = metadata.PropertyInfo.GetValue(value);
                    if (v != null)
                    {
                        ResolveInternal(metadata, dictionary, v, parent);
                    }
                }
                else
                {
                    ResolveInternal(metadata, dictionary, value);
                }
            }
        }

        // Per request parameter context resolver
        public ResolvedContentResult Resolve(List<ProxyModelMetadata> parameters,
            HttpMethod httpMethod,
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

            if (httpMethod == HttpMethod.Get)
            {
                // Ref type parameter resolver
                if (parameters.Count == 1 && parameters[0].IsReferenceType)
                {
                    var modelMetadata = MetadataProvider.GetMetadataForType(args[0].GetType());

                    var obj = args[0].ToDictionary();

                    // TODO Gencebay
                    // values.Merge(obj, true);
                    // return values;
                }

                if (parameters.Count > 1 && parameters.Any(x => x.IsReferenceType))
                {
                    throw new ArgumentOutOfRangeException($"Methods marked with HTTP GET can take only one reference type parameter at the same time.");
                }
            }

            // TODO Gencebay
            // values.MergeArgs(args, parameters, isMultiPartFormData);
            // return values;

            return new ResolvedContentResult(dictionary);
        }
    }
}
