using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class DictionaryExtensions
    {
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

        internal static string ToQueryString(this Uri baseUrl, IDictionary<string, string> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return baseUrl.AbsoluteUri;

            // encode and create url
            return AddQueryString(baseUrl.AbsoluteUri, dictionary);
        }

        internal static void Merge(this IDictionary<string, string> instance, IDictionary<string, string> from, bool replaceExisting)
        {
            foreach (KeyValuePair<string, string> entry in from)
            {
                if (replaceExisting || !instance.ContainsKey(entry.Key))
                {
                    instance[entry.Key] = entry.Value;
                }
            }
        }
    }
}
