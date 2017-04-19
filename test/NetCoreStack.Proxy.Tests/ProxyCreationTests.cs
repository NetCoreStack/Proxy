using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreStack.Proxy.Tests
{
    public class ProxyCreationTests
    {
        protected IServiceProvider Resolver { get; }
        protected IConfigurationRoot Configuration { get; }
        protected TestHostingServer<ServerApp.Startup> TestServer { get; }

        private void RegisterServices(IServiceCollection services)
        {
            var httpClientAccessor = new Mock<IHttpClientAccessor>();
            httpClientAccessor.Setup(x => x.HttpClient).Returns(TestServer.Client);
            services.AddSingleton<IHttpClientAccessor>(httpClientAccessor.Object);
        }

        public ProxyCreationTests()
        {
            TestServer = new TestHostingServer<ServerApp.Startup>();
            Resolver = TestHelper.GetServiceProvider(RegisterServices);
            Configuration = TestHelper.Configuration;
        }

        [Fact]
        public void VoidCall()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            guidelineApi.VoidOperation();
        }

        [Fact]
        public async Task TaskCallHttpPostWithReferenceTypeParameter()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var simpleModel = new SimpleModel
            {
                Name = nameof(TaskCallHttpPostWithReferenceTypeParameter),
                Date = DateTime.Now,
                Value = "<<string>>"
            };
            await guidelineApi.TaskActionPost(simpleModel);
        }

        [Fact]
        public async Task TaskCallHttpGetWithReferenceTypeParameter()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var simpleModel = new SimpleModel
            {
                Name = nameof(TaskCallHttpGetWithReferenceTypeParameter),
                Date = DateTime.Now,
                Value = "<<string>>"
            };
            await guidelineApi.GetWithReferenceType(simpleModel);
        }

        [Fact]
        public async Task GenericTaskResultCall()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var items = await guidelineApi.GetPostsAsync();
            Assert.True(items.GetType() == typeof(List<Post>));
        }

        [Fact]
        public async Task GetCollectionStream()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var items = await guidelineApi.GetCollectionStream();
            Assert.True(items.GetType() == typeof(CollectionResult<Post>));
        }
    }
}
