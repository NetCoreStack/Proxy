﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NetCoreStack.Proxy.Extensions
{
    public static class ContentModelBindingContextExtensions
    {
        public static ResolvedContentResult GetResolvedContentResult(this ContentModelBindingContext bindingContext, HttpMethod httpMethod)
        {
            return bindingContext.ModelContentResolver.Resolve(httpMethod, bindingContext.Parameters, bindingContext.IsMultiPartFormData, bindingContext.Args);
        }

        public static void TrySetContent(this ContentModelBindingContext bindingContext, HttpRequestMessage httpRequest)
        {
            if (bindingContext.ContentResult != null && bindingContext.ContentResult.IsContentSet)
            {
                httpRequest.Content = bindingContext.ContentResult.Content;
            }
        }

        public static void TryUpdateUri(this ContentModelBindingContext bindingContext, Dictionary<string, string> dictionary)
        {
            if (bindingContext.UriDefinition != null)
            {
                var uriBuilder = new UriBuilder(bindingContext.Uri.ToQueryString(dictionary));
                bindingContext.UriDefinition.UriBuilder = uriBuilder;
            }            
        }
    }
}