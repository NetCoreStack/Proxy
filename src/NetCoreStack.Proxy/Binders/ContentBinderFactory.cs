using System;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public static class ContentBinderFactory
    {
        static ContentBinderFactory()
        {

        }

        public static IContentModelBinder GetContentModelBinder(HttpMethod httpMethod)
        {
            if (httpMethod == HttpMethod.Get)
            {
                return new HttpGetContentBinder();
            }

            if (httpMethod == HttpMethod.Post)
            {
                return new HttpPostContentBinder();
            }

            if (httpMethod == HttpMethod.Put)
            {
                return new HttpPutContentBinder();
            }

            if (httpMethod == HttpMethod.Delete)
            {
                return new HttpDeleteContentBinder();
            }

            throw new NotImplementedException($"{httpMethod.Method} is not supported or not implemented yet.");
        }
    }
}