using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace NetCoreStack.Proxy.Tests
{
    public static class TestHelper
    {
        public static HttpContext HttpContext { get; set; }

        public static IConfiguration Configuration { get; set; }

        private static void CreateHttpContext(IServiceProvider serviceProvider)
        {
            var contentType = "application/json; charset=utf-8";
            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();
            var items = new Dictionary<object, object>();
            var cookies = new Dictionary<string, string>();
            var responseHeaders = new HeaderDictionary();

            var headers = new HeaderDictionary();
            headers.Add(ContextBaseExtensions.ClientUserAgentHeader, new StringValues("Chrome/63.0.3239.84 Safari/537.36"));

            request.SetupGet(x => x.Cookies).Returns(new RequestCookieCollection(cookies));
            request.SetupGet(r => r.Headers).Returns(headers);
            request.SetupGet(f => f.ContentType).Returns(contentType);
            response.SetupGet(r => r.Headers).Returns(responseHeaders);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.RequestServices).Returns(serviceProvider);
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            httpContext.SetupGet(c => c.Items).Returns(items);
            httpContext.Setup(c => c.Response).Returns(response.Object);

            HttpContext = httpContext.Object;
        }

        static TestHelper()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
            services.AddSingleton<IHttpContextAccessor>(resolver => new HttpContextAccessor
            {
                HttpContext = HttpContext
            });
            
            var env = new HostingEnvironment
            {
                ContentRootPath = Directory.GetCurrentDirectory()
            };

            services.AddSingleton(env);

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            services.AddNetCoreProxy(Configuration, options =>
            {
                options.DefaultHeaders.Add("X-NetCoreStack-Header", "ProxyHeaderValue");
                options.RegisterFilter<CustomProxyContextFilter>();
                options.Register<IGuidelineApi>();
                options.Register<IFileProxyApi>();
                options.Register<IConsulApi>();

                options.CultureFactory = () =>
                {
                    var thread = System.Threading.Thread.CurrentThread;
                    return thread.CurrentCulture;
                };
            });

            CreateHttpContext(services.BuildServiceProvider());
        }

        public static FormFile GetFormFile(string name, string fileName = "")
        {
            var content = "some text content";
            var bytes = Encoding.UTF8.GetBytes(content);
            var length = bytes.Length;
            var ms = new MemoryStream(bytes);

            fileName = string.IsNullOrEmpty(fileName) ? "some_text_file.txt" : fileName;
            var formFile = new FormFile(ms, 0, length, name, fileName)
            {
                Headers = new HeaderDictionary
                {
                    [HeaderNames.ContentType] = "text/plain",
                    [HeaderNames.ContentDisposition] = $"form-data; name=\"{name}\"; filename=\"{fileName}\""
                }
            };

            return formFile;
        }
    }
}