using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class DictionaryExtensions
    {
        private static string TypeParser(KeyValuePair<string, object> selector)
        {
            if (selector.Value == null)
                return null;

            if (selector.Value != null)
            {
                if (selector.Value is DateTime)
                {
                    return ((DateTime)selector.Value).ToString(CultureInfo.InvariantCulture);
                }
            }

            return selector.Value.ToString();
        }

        internal static void Merge(this IDictionary<string, object> instance, IDictionary<string, object> from, bool replaceExisting)
        {
            foreach (KeyValuePair<string, object> entry in from)
            {
                if (replaceExisting || !instance.ContainsKey(entry.Key))
                {
                    instance[entry.Key] = entry.Value;
                }
            }
        }

        internal static void MergeArgs(this IDictionary<string, object> dictionary, 
            object[] args, 
            ProxyParameterDescriptor[] parameters,
            bool isMultiPartFormData)
        {
            if (args.Length == 0)
                return;

            if (!isMultiPartFormData)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    dictionary.Add(parameters[i].Name, args[i]);
                }

                return;
            }

            // Multipart form data context preparing
            for (int i = 0; i < parameters.Length; i++)
            {
                foreach (var prop in parameters[i].Properties)
                {
                    string name = prop.Key;
                    PropertyContentTypeInfo contentTypeInfo = prop.Value;

                    var parameterContext = new PropertyContext
                    {
                        Name = name,
                        Value = contentTypeInfo.PropertyInfo.GetValue(args[i]),
                        PropertyContentType = contentTypeInfo.PropertyContentType
                    };

                    dictionary.Add(name, parameterContext);
                }
            }
        }

        internal static string ToQueryString(this Uri baseUrl, IDictionary<string, object> objectDics)
        {
            if (objectDics == null)
                return baseUrl.AbsoluteUri;

            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> entry in objectDics)
            {
                // ignore null parameters for QueryString
                if (entry.Value != null)
                {
                    dict.Add(entry.Key, TypeParser(entry));
                }
            }
            // encode and create url
            return QueryHelpers.AddQueryString(baseUrl.AbsoluteUri, dict);
        }
    }
}
