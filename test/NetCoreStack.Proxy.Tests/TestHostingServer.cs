using Microsoft.AspNetCore.Hosting;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Reflection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Net.Http;

namespace NetCoreStack.Proxy.Tests
{
    public class TestHostingServer<TStartup>
    {
        private readonly TestServer _server;

        public HttpClient Client { get; }

        public TestHostingServer()
            : this("test")
        {

        }

        protected TestHostingServer(string solutionRelativePath)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            var contentRoot = SolutionPathUtility.GetProjectPath(solutionRelativePath, startupAssembly);

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServices)
                .UseEnvironment(EnvironmentName.Development)
                .UseUrls("http://*:5005")
                .UseStartup(typeof(TStartup));

            _server = new TestServer(builder);
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost:5005");
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
            // Inject a custom application part manager. Overrides AddMvcCore() because that uses TryAdd().
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(typeof(TStartup).GetTypeInfo().Assembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
        }
    }
}
