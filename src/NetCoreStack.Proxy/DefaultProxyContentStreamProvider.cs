using Microsoft.AspNetCore.Http;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Extensions;
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
        public ProxyMetadataProvider MetadataProvider { get; }
        public IModelContentResolver ContentResolver { get; }

        public DefaultProxyContentStreamProvider(ProxyMetadataProvider metadataProvider, IModelContentResolver contentResolver)
        {
            MetadataProvider = metadataProvider;
            ContentResolver = contentResolver;
        }

        private void AddFile(string key, MultipartFormDataContent multipartFormDataContent, IFormFile formFile)
        {
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(formFile.ContentType);
                fileContent.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition);
                multipartFormDataContent.Add(fileContent, key, formFile.FileName);
            }
        }

        protected virtual byte[] Serialize(object value)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
        }

        protected virtual StringContent SerializeToString(object value)
        {
            return new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
        }

        protected virtual MultipartFormDataContent GetMultipartFormDataContent(ResolvedContentResult contentResult)
        {
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> entry in contentResult.Dictionary)
            {
                multipartFormDataContent.Add(new StringContent(entry.Value), entry.Key);    
            }

            if (contentResult.Files != null)
            {
                foreach (KeyValuePair<string, IFormFile> entry in contentResult.Files)
                {
                    AddFile(entry.Key, multipartFormDataContent, entry.Value);
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
            RequestDescriptor requestContext,
            IDictionary<string, string> argsDic,
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

        public async Task CreateRequestContentAsync(RequestDescriptor requestContext, 
            HttpRequestMessage request, 
            ProxyMethodDescriptor descriptor,
            ProxyUriDefinition proxyUriDefinition)
        {
            await Task.CompletedTask;

            var httpMethod = descriptor.HttpMethod;
            var isMultiPartFormData = descriptor.IsMultiPartFormData;
            var uriBuilder = proxyUriDefinition.UriBuilder;
            ResolvedContentResult result = ContentResolver.Resolve(descriptor.Parameters, httpMethod, isMultiPartFormData, requestContext.Args);
            var argsDic = result.Dictionary;
            var argsCount = argsDic.Count;
            var keys = new List<string>(argsDic.Keys);
            if (httpMethod == HttpMethod.Post)
            {
                if (isMultiPartFormData)
                {
                    request.Content = GetMultipartFormDataContent(result);
                    return;
                }

                if (argsCount == 1)
                    request.Content = SerializeToString(argsDic.First().Value);
                else
                    request.Content = SerializeToString(argsDic);

                return;
            }
            else if(httpMethod == HttpMethod.Put)
            {
                // TODO Gencebay
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
            if (httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Delete)
            {
                EnsureTemplate(descriptor, proxyUriDefinition, requestContext, argsDic, keys);
                // TODO Gencebay
                // request.RequestUri = QueryStringResolver.Parse(uriBuilder, argsDic);
            }
        }
    }
}
