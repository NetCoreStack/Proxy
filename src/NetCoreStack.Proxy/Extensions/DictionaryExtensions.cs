using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class DictionaryExtensions
    {
        //private static string TypeParser(KeyValuePair<string, object> selector)
        //{
        //    if (selector.Value == null)
        //        return null;

        //    if (selector.Value != null)
        //    {
        //        if (selector.Value is DateTime)
        //        {
        //            return ((DateTime)selector.Value).ToString(CultureInfo.InvariantCulture);
        //        }
        //    }

        //    return selector.Value.ToString();
        //}

        // Copyright (c) .NET Foundation. All rights reserved.
        // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
        private static string AddQueryString(string uri, IEnumerable<KeyValuePair<string, string>> queryString)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            var anchorIndex = uri.IndexOf('#');
            var uriToBeAppended = uri;
            var anchorText = "";
            // If there is an anchor, then the query string must be inserted before its first occurance.
            if (anchorIndex != -1)
            {
                anchorText = uri.Substring(anchorIndex);
                uriToBeAppended = uri.Substring(0, anchorIndex);
            }

            var queryIndex = uriToBeAppended.IndexOf('?');
            var hasQuery = queryIndex != -1;

            var sb = new StringBuilder();
            sb.Append(uriToBeAppended);
            foreach (var parameter in queryString)
            {
                sb.Append(hasQuery ? '&' : '?');
                sb.Append(UrlEncoder.Default.Encode(parameter.Key));
                sb.Append('=');
                sb.Append(UrlEncoder.Default.Encode(parameter.Value));
                hasQuery = true;
            }

            sb.Append(anchorText);
            return sb.ToString();
        }

        // Copyright (c) .NET Foundation. All rights reserved.
        // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
        private static string AddQueryString(string uri, IDictionary<string, string> queryString)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            return AddQueryString(uri, (IEnumerable<KeyValuePair<string, string>>)queryString);
        }

        //private static void AddToDictionary(IDictionary<string, string> source, ProxyModelMetadata metadata, object instance)
        //{
        //    do
        //    {
        //        foreach (var modelMetadata in metadata.Properties)
        //        {
        //            var prefix = modelMetadata.ContainerType?.Name;
        //            var propertyName = modelMetadata.PropertyName;
        //            var key = !string.IsNullOrEmpty(prefix) ? $"{prefix}.{propertyName}" : propertyName;

        //            if (modelMetadata.IsSimpleType)
        //            {
        //                var value = Convert.ToString(instance, CultureInfo.InvariantCulture);
        //                if (!string.IsNullOrEmpty(value))
        //                {
        //                    source.Add(key, value);
        //                }
        //            }
        //        }
        //    }
        //    while (metadata.Properties.Any());
        //}

        //internal static void Merge(this IDictionary<string, object> instance, IDictionary<string, object> from, bool replaceExisting)
        //{
        //    foreach (KeyValuePair<string, object> entry in from)
        //    {
        //        if (replaceExisting || !instance.ContainsKey(entry.Key))
        //        {
        //            instance[entry.Key] = entry.Value;
        //        }
        //    }
        //}

        //internal static void Merge(this IDictionary<string, PropertyContext> instance, IDictionary<string, PropertyContext> from, bool replaceExisting)
        //{
        //    foreach (KeyValuePair<string, PropertyContext> entry in from)
        //    {
        //        if (replaceExisting || !instance.ContainsKey(entry.Key))
        //        {
        //            instance[entry.Key] = entry.Value;
        //        }
        //    }
        //}

        //internal static void MergeArgs(this IDictionary<string, PropertyContext> dictionary,
        //    object[] args,
        //    List<ProxyModelMetadata> parameters,
        //    bool isMultiPartFormData)
        //{
        //    if (args.Length == 0)
        //        return;

        //    if (!isMultiPartFormData)
        //    {
        //        for (int i = 0; i < parameters.Count; i++)
        //        {
        //            var modelMetadata = parameters[i];
        //            dictionary.Add(modelMetadata.PropertyName, new PropertyContext { ModelMetadata = modelMetadata, Value = args[i] });
        //        }

        //        return;
        //    }

        //    // Multipart form data context
        //    for (int i = 0; i < parameters.Count; i++)
        //    {
        //        var modelMetadata = parameters[i];
        //        foreach (var prop in parameters[i].Properties)
        //        {
        //            var parameterContext = new PropertyContext
        //            {
        //                ModelMetadata = prop,
        //                // TODO Gencebay
        //                // Value = contentTypeInfo.PropertyInfo.GetValue(args[i]),
        //            };
        //            // TODO Gencebay
        //            // dictionary.Add(name, parameterContext);
        //        }
        //    }
        //}

        internal static string ToQueryString(this Uri baseUrl, IDictionary<string, string> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return baseUrl.AbsoluteUri;

            // encode and create url
            return AddQueryString(baseUrl.AbsoluteUri, dictionary);
        }
    }
}
