using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using System.IO;
using NetCoreStack.Proxy.Test.Contracts;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Internal;

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
            services.AddSingleton(new ApplicationPartManager());
            services.AddSingleton<DiagnosticSource>(new DiagnosticListener("Microsoft.AspNetCore.Mvc"));

            services.AddMvc();

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddTransient<ILoggerFactory, LoggerFactory>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(resolver =>
            {
                return new HttpContextAccessor { HttpContext = HttpContext };
            });

            var env = new HostingEnvironment();
            env.ContentRootPath = Directory.GetCurrentDirectory();

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
                options.Register<IGuidelineApi>();
            });

            var serviceProvider = services.BuildServiceProvider();
            CreateHttpContext(serviceProvider);
            return serviceProvider;
        }
    }
}
