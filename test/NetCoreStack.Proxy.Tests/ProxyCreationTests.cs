using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
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
        public async Task TaskOperationTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var task = guidelineApi.TaskOperation();
            await task;
        }

        [Fact]
        public async Task GetComplexTypeTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var task = guidelineApi.GetComplexType(TypesModelHelper.GetComplexTypeModel());
            await task;
        }

        [Fact]
        public async Task PrimitiveReturnTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var task = guidelineApi.PrimitiveReturn(int.MaxValue, "some string", long.MaxValue, DateTime.Now);
            await task;
        }

        [Fact]
        public async Task GetWithComplexReferenceTypeTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var task = guidelineApi.GetWithComplexReferenceType(new CollectionRequest {
                Draw = 1,
                Filters = "",
                Length = 10,
                Metadata = typeof(ComplexTypeModel).FullName,
                Start = 0,
                Search = null,
                Text = "",
                Columns = new List<Column>
                {
                    new Column
                    {
                        Composer = "",
                        Data = nameof(ComplexTypeModel.Int),
                        Meta = typeof(int).Name,
                        Search = null,
                        Searchable = true,
                        Orderable = true
                    },
                    new Column
                    {
                        Composer = "",
                        Data = nameof(ComplexTypeModel.String),
                        Meta = typeof(String).Name,
                        Search = null,
                        Searchable = false,
                        Orderable = false
                    }
                },
                Order = new List<OrderDescriptor>
                {
                    new OrderDescriptor
                    {
                        ColumnIndex = 0,
                        Direction = ListSortDirection.Ascending
                    }
                }
            });

            await task;
        }

        [Fact]
        public async Task TaskCallHttpPostWithReferenceTypeParameterTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskActionPost(TypesModelHelper.GetComplexTypeModel());
        }

        [Fact]
        public async Task GenericTaskResultCallTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var items = await guidelineApi.GetEnumerableModels();
            Assert.True(items != null);
            Assert.True(items.Count() == 4);
        }

        [Fact]
        public async Task GetCollectionStreamTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var collection = await guidelineApi.GetCollectionStreamTask();
            Assert.True(collection != null);
            Assert.True(collection.Data.Count() == 5);
        }

        [Fact]
        public async Task TaskActionPutTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskActionPut(1, new SampleModel
            {
                Date = DateTime.Now,
                String = "Sample model"
            });

            Assert.True(true);
        }

        [Fact]
        public async Task TaskActionDeleteTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskActionDelete(1);
            Assert.True(true);
        }
    }
}
