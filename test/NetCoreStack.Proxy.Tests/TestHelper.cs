using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetCoreStack.Proxy.Tests
{
    public static class TestHelper
    {
        public static HttpContext HttpContext { get; set; }

        public static IConfigurationRoot Configuration { get; set; }

        public static void CreateHttpContext(IServiceProvider resolver)
        {
            var contentType = "application/json";
            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();
            var items = new Dictionary<object, object>();
            var cookies = new Dictionary<string, string>();
            var headers = new Mock<IHeaderDictionary>();
            var responseHeaders = new HeaderDictionary();

            request.SetupGet(x => x.Cookies).Returns(new RequestCookieCollection(cookies));
            request.SetupGet(r => r.Headers).Returns(headers.Object);
            request.SetupGet(f => f.ContentType).Returns(contentType);
            response.SetupGet(r => r.Headers).Returns(responseHeaders);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.RequestServices).Returns(resolver);
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            httpContext.SetupGet(c => c.Items).Returns(items);
            httpContext.Setup(c => c.Response).Returns(response.Object);

            HttpContext = httpContext.Object;
        }

        public static IServiceProvider GetServiceProvider(Action<IServiceCollection> setup = null)
        {
            var services = new ServiceCollection();
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

            setup?.Invoke(services);

            services.AddNetCoreProxy(Configuration, options =>
            {
                options.DefaultHeaders.Add("X-NetCoreStack-Header", "ProxyHeaderValue");
                options.Register<IGuidelineApi>();
                options.Register<IConsulApi>();
            });

            var serviceProvider = services.BuildServiceProvider();
            CreateHttpContext(serviceProvider);
            return serviceProvider;
        }
    }
}
