using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NetCoreStack.Proxy.Extensions
{
    public static class ContentModelBindingContextExtensions
    {
        public static void TrySetContent(this ContentModelBindingContext bindingContext, HttpRequestMessage httpRequest)
        {
            if (bindingContext.ContentResult != null && bindingContext.ContentResult.IsContentSet)
            {
                httpRequest.Content = bindingContext.ContentResult.Content;
            }
        }

        public static void TryUpdateUri(this ContentModelBindingContext bindingContext, Dictionary<string, string> dictionary)
        {
            if (bindingContext.UriBuilder != null)
            {
                var uriBuilder = new UriBuilder(bindingContext.Uri.ToQueryString(dictionary));
                bindingContext.UriBuilder = uriBuilder;
            }            
        }
    }
}
