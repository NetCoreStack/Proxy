using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace NetCoreStack.Proxy
{
    public abstract class BodyContentBinder : ContentModelBinder
    {
        protected virtual void AddFile(string key, MultipartFormDataContent multipartFormDataContent, IFormFile formFile)
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

        protected virtual MultipartFormDataContent GetMultipartFormDataContent(ModelDictionaryResult contentResult)
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

    }
}
