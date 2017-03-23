using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

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

        public static void MergeArgs(this IDictionary<string, object> dictionary, object[] args, ParameterDescriptor[] parameters)
        {
            if (args.Length == 0)
                return;

            for (int i = 0; i < parameters.Length; i++)
            {
                dictionary.Add(parameters[i].Name, args[i]);
            }
        }

        public static string ToQueryString(this Uri baseUrl, IDictionary<string, object> objectDics)
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
