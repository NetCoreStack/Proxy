using Microsoft.AspNetCore.Http;
using NetCoreStack.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace NetCoreStack.Proxy
{
    public abstract class BodyContentBinder : ContentModelBinder
    {
		private const string formData = "form-data";
        protected IModelSerializer ModelSerializer { get; }

        public BodyContentBinder(HttpMethod httpMethod, IModelSerializer modelSerializer)
            :base(httpMethod)
        {
            ModelSerializer = modelSerializer;
        }

       protected virtual void AddFile(string key, MultipartFormDataContent multipartFormDataContent, IFormFile formFile)
        {
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType) { CharSet = Encoding.UTF8.WebName };
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue(formData)
                {
                    Name = formFile.Name,
                    FileName = formFile.FileName,
                    Size = formFile.Length
                };
                
                multipartFormDataContent.Add(fileContent, key, formFile.FileName);
            }
        }

        protected virtual byte[] Serialize(object value)
        {
            return Encoding.UTF8.GetBytes(ModelSerializer.Serialize(value));
        }

        protected virtual StringContent SerializeToString(ContentType contentType, ArraySegment<object> args)
        {
            var mediaType = "application/json";
            if (contentType == ContentType.Xml)
                mediaType = "application/xml";

            if (args.Count == 1)
            {
                return new StringContent(ModelSerializer.Serialize(args.First()), Encoding.UTF8, mediaType);
            }

            return new StringContent(ModelSerializer.Serialize(args), Encoding.UTF8, mediaType);
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

        protected virtual FormUrlEncodedContent GetUrlEncodedContent(ModelDictionaryResult contentResult)
        {
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(contentResult.Dictionary);
            return formUrlEncodedContent;
        }

        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            EnsureTemplateResult ensureTemplateResult = EnsureTemplate(bindingContext);
            if (ensureTemplateResult.BindingCompleted)
                return;

            var parameterOffset = ensureTemplateResult.ParameterOffset;
            ModelDictionaryResult result = bindingContext.ModelContentResolver.Resolve(bindingContext.Parameters,
                bindingContext.Args,
                parameterOffset,
                ensureTemplateResult.IgnoreModelPrefix);

            var contentType = bindingContext.ContentType;
            if (contentType == ContentType.MultipartFormData)
            {
                var content = GetMultipartFormDataContent(result);
                bindingContext.ContentResult = ContentModelBindingResult.Success(content);
                return;
            }

            if (contentType == ContentType.FormUrlEncoded)
            {
                var content = GetUrlEncodedContent(result);
                bindingContext.ContentResult = ContentModelBindingResult.Success(content);
                return;
            }

            var remainingParameterCount = bindingContext.ArgsLength - parameterOffset;
            if (remainingParameterCount <= 0)
            {
                // all parameters are resolved as Key - Url
                return;
            }

            var segments = new ArraySegment<object>(bindingContext.Args, parameterOffset, remainingParameterCount);
            bindingContext.ContentResult = ContentModelBindingResult.Success(SerializeToString(contentType, segments));
        }
    }
}