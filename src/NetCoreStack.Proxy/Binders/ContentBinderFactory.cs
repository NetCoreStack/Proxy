using NetCoreStack.Contracts;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreStack.Proxy
{
    public static class ContentBinderFactory
    {
        public static IContentModelBinder GetContentModelBinder(IServiceProvider serviceProvider, 
            HttpMethod httpMethod, 
            ContentType contentType)
        {
            IModelSerializer modelSerializer = null;
            if (contentType == ContentType.Xml)
            {
                modelSerializer = serviceProvider.GetService<IModelXmlSerializer>();
            }
            else
            {
                modelSerializer = serviceProvider.GetService<IModelJsonSerializer>();
            }

            if (httpMethod == HttpMethod.Get)
            {
                return new HttpGetContentBinder(httpMethod);
            }

            if (httpMethod == HttpMethod.Post)
            {
                return new HttpPostContentBinder(httpMethod, modelSerializer);
            }

            if (httpMethod == HttpMethod.Put)
            {
                return new HttpPutContentBinder(httpMethod, modelSerializer);
            }

            if (httpMethod == HttpMethod.Delete)
            {
                return new HttpDeleteContentBinder(httpMethod);
            }

            throw new NotImplementedException($"{httpMethod.Method} is not supported or not implemented yet.");
        }
    }
}