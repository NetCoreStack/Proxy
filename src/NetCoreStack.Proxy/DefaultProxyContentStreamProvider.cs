using Microsoft.AspNetCore.Http;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Extensions;
using NetCoreStack.Proxy.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public class DefaultProxyContentStreamProvider : IProxyContentStreamProvider
    {
        protected virtual byte[] Serialize(object value)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
        }

        protected virtual StringContent SerializeToString(object value)
        {
            return new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
        }

        protected virtual MultipartFormDataContent GetMultipartFormDataContent(IDictionary<string,object> values)
        {
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, object> entry in values)
            {
                var parameterContext = entry.Value as PropertyContext;
                if (parameterContext != null)
                {
                    if (parameterContext.PropertyContentType == PropertyContentType.Multipart)
                    {
                        IFormFile formFile = parameterContext.Value as IFormFile;
                        if (formFile != null)
                        {
                            using (var ms = new MemoryStream())
                            {
                                formFile.CopyTo(ms);
                                var fileContent = new ByteArrayContent(ms.ToArray());
                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(formFile.ContentType);
                                fileContent.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition);
                                multipartFormDataContent.Add(fileContent, entry.Key, formFile.FileName);
                            }
                        }                                             
                    }
                    else
                    {
                        var stringContent = parameterContext.Value?.ToString();
                        if (!string.IsNullOrEmpty(stringContent))
                        {
                            multipartFormDataContent.Add(new StringContent(stringContent), entry.Key);
                        }
                    }
                }                
            }

            return multipartFormDataContent;
        }

        protected virtual HttpContent CreateHttpContent(object value)
        {
            HttpContent content;

            if (value.GetType() == typeof(byte[]))
            {
                content = new ByteArrayContent((value as byte[]) ?? new byte[0]);
            }
            else if (value.GetType() == typeof(Stream))
            {
                content = new StreamContent((value as Stream) ?? new MemoryStream());
            }
            else
            {
                content = new ByteArrayContent(Serialize(value));
            }

            return content;
        }

        protected virtual void EnsureTemplate(ProxyMethodDescriptor descriptor, 
            ProxyUriDefinition proxyUriDefinition,
            RequestContext requestContext,
            IDictionary<string, object> argsDic,
            List<string> keys)
        {
            if (descriptor.Template.HasValue())
            {
                if (proxyUriDefinition.HasParameter)
                {
                    for (int i = 0; i < proxyUriDefinition.ParameterParts.Count; i++)
                    {
                        var key = keys[i];
                        proxyUriDefinition.UriBuilder.Path += ($"/{WebUtility.UrlEncode(requestContext.Args[i]?.ToString())}");
                        argsDic.Remove(key);
                    }
                }
            }
        }

        public async Task CreateRequestContentAsync(RequestContext requestContext, 
            HttpRequestMessage request, 
            ProxyMethodDescriptor descriptor,
            ProxyUriDefinition proxyUriDefinition)
        {
            await Task.CompletedTask;

            var uriBuilder = proxyUriDefinition.UriBuilder;
            var argsDic = descriptor.Resolve(requestContext.Args);
            var argsCount = argsDic.Count;
            var keys = new List<string>(argsDic.Keys);
            if (descriptor.HttpMethod == HttpMethod.Post)
            {
                if (descriptor.IsMultiPartFormData)
                {
                    request.Content = GetMultipartFormDataContent(argsDic);
                    return;
                }

                if (argsCount == 1)
                    request.Content = SerializeToString(argsDic.First().Value);
                else
                    request.Content = SerializeToString(argsDic);

                return;
            }
            else if(descriptor.HttpMethod == HttpMethod.Put)
            {
                EnsureTemplate(descriptor, proxyUriDefinition, requestContext, argsDic, keys);
                argsCount = argsDic.Count;
                request.RequestUri = uriBuilder.Uri;
                if (argsCount == 1)
                {
                    request.Content = SerializeToString(argsDic.First().Value);
                }
                else if(argsCount == 2)
                {
                    var firstParameter = argsDic[keys[0]];
                    var secondParameter = argsDic[keys[1]];

                    // PUT Request first parameter should be Id or Key
                    if (firstParameter.GetType().IsPrimitive())
                    {
                        uriBuilder.Query += string.Format("&{0}={1}", keys[0], firstParameter);
                    }

                    request.RequestUri = uriBuilder.Uri;
                    request.Content = SerializeToString(secondParameter);
                }
            }
            if (descriptor.HttpMethod == HttpMethod.Get || descriptor.HttpMethod == HttpMethod.Delete)
            {
                EnsureTemplate(descriptor, proxyUriDefinition, requestContext, argsDic, keys);
                request.RequestUri = QueryStringResolver.Parse(uriBuilder, argsDic);
            }
        }
    }
}
