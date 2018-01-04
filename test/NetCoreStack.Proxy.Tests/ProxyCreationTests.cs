using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreStack.Proxy.Tests
{
    public class ProxyCreationTests
    {
        protected IServiceProvider Resolver { get; }
        protected IConfigurationRoot Configuration { get; }

        public ProxyCreationTests()
        {
            Resolver = TestHelper.HttpContext.RequestServices;
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
            var simpleModel = new SampleModel
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
            var simpleModel = new SampleModel
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
            var items = await guidelineApi.GetEnumerableModels();
            Assert.True(items != null);
            Assert.True(items.Count() == 4);
        }

        [Fact]
        public async Task GetCollectionStream()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var collection = await guidelineApi.GetCollectionStreamTask();
            Assert.True(collection != null);
            Assert.True(collection.Data.Count() == 4);
        }

        [Fact]
        public async Task TaskActionPut()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskActionPut(1, new SampleModel
            {
                Date = DateTime.Now,
                Name = "Sample model",
                Value = "{foo: bar}"
            });

            Assert.True(true);
        }

        [Fact]
        public async Task TaskActionDelete()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskActionDelete(1);
            Assert.True(true);
        }
    }
}
