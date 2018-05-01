using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreStack.Proxy.Tests
{
    public class ProxyCreationTests
    {
        private readonly string _someKey = "248fd6db0ae44ec48169fa2391b067da";

        protected IServiceProvider Resolver { get; }
        protected IConfiguration Configuration { get; }

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
            var task = guidelineApi.GetWithComplexReferenceType(new CollectionRequest
            {
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
        [Trait("Category", "Group")]
        public async Task TaskCallHttpPostWithReferenceTypeParameterTest()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskComplexTypeModel(TypesModelHelper.GetComplexTypeModel());
        }

        [Fact]
        [Trait("Category", "Group")]
        public async Task TaskActionBarMultipartFormDataTest()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskActionBarMultipartFormData(new Bar
            {
                String = "Bar string value!",
                someint = 6,
                SomeEnum = SomeEnum.Value2,
                Foo = new Foo { IEnumerableInt = new[] { 1, 3, 5, 7, 9 }, String = "Foo string value!" }
            });
        }

        [Fact]
        [Trait("Category", "Group")]
        public async Task TaskActionBarSimpleXml()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskActionBarSimpleXml(new BarSimple
            {
                String = "Bar string value!",
                someint = 6,
                SomeEnum = SomeEnum.Value2,
            });
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
        public async Task GetSingleFileWithKeyTemplate()
        {
            var guidelineApi = Resolver.GetService<IFileProxyApi>();
            byte[] bytes = await guidelineApi.GetFileAsync("5a6ee0791653ff2348f1cd32", "pdf-sample.pdf");
            Assert.True(true);
        }

        [Fact]
        public async Task TaskSingleFileModel()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();

            var model = new SingleFileModel
            {
                File = TestHelper.GetFormFile("file")
            };

            await guidelineApi.TaskSingleFileModel(model);
            Assert.True(true);
        }

        [Fact]
        public async Task UploadAsyncTemplateTest()
        {
            var fileProxyApi = Resolver.GetService<IFileProxyApi>();

            var model = new FileProxyUploadContext
            {
                Directory = "proxy-fs/123456",
                Files = new[] { TestHelper.GetFormFile("files", "file1.txt"), TestHelper.GetFormFile("files", "file2.txt") }
            };

            await fileProxyApi.UploadAsync(model);
            Assert.True(true);
        }

        [Fact]
        public async Task TaskKeyAndSingleFileModel()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();

            var model = new SingleFileModel
            {
                File = TestHelper.GetFormFile("file")
            };

            await guidelineApi.TaskKeyAndSingleFileModel(_someKey, model);
            Assert.True(true);
        }

        [Fact]
        public async Task TaskKeyAndSingleFileAndPropsModel()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();

            var model = new SingleFileAndPropsModel
            {
                Int = 6,
                String = "Some string value!",
                File = TestHelper.GetFormFile("file")
            };

            await guidelineApi.TaskKeyAndSingleFileAndPropsModel(_someKey, model);
            Assert.True(true);
        }

        [Fact]
        public async Task TaskEnumerableFileModel()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();

            var model = new EnumerableFileModel
            {
                Files = new[] { TestHelper.GetFormFile("files", "file1.txt"), TestHelper.GetFormFile("files", "file2.txt") }
            };

            await guidelineApi.TaskEnumerableFileModel(model);
            Assert.True(true);
        }

        [Fact]
        public async Task TaskKeyAndEnumerableFileModel()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();

            var model = new EnumerableFileModel
            {
                Files = new[] { TestHelper.GetFormFile("files", "file1.txt"), TestHelper.GetFormFile("files", "file2.txt") }
            };

            await guidelineApi.TaskKeyAndEnumerableFileModel(_someKey, model);
            Assert.True(true);
        }

        [Fact]
        public async Task CreateOrUpdateNoBodyKeyTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var result = await guidelineApi.CreateOrUpdateKey(_someKey);
            Assert.True(result);
        }

        [Fact]
        public async Task CreateOrUpdateKeyTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            var result = await guidelineApi.CreateOrUpdateKey(_someKey, new Bar
            {
                String = "Bar string value!",
                someint = 6,
                SomeEnum = SomeEnum.Value2,
                Foo = new Foo { IEnumerableInt = new[] { 1, 3, 5, 7, 9 }, String = "Foo string value!" }
            });
            Assert.True(result);
        }

        [Fact]
        public async Task TaskActionDeleteTest()
        {
            var guidelineApi = Resolver.GetService<IGuidelineApi>();
            await guidelineApi.TaskActionDelete(1);
            Assert.True(true);
        }

        [Fact]
        public async Task SelfApiOperation()
        {
            var selfApi = Resolver.GetService<ISelfApi>();
            await selfApi.Operation(new Baz { String = "Some string", Self = new Self { Int = 7 } });
            Assert.True(true);
        }

        [Fact]
        public async Task SelfApiOperationCollection()
        {
            var selfApi = Resolver.GetService<ISelfApi>();
            await selfApi.OperationCollection(new Baz2
            {
                String = "Some string",
                SelfCollection = new List<SelfCollection> { new SelfCollection { String = "Collection string" } }
            });

            Assert.True(true);
        }

        [Fact]
        public async Task SelfApiWithThisOperation()
        {
            var selfApi = Resolver.GetService<ISelfApi>();
            var baz = new Baz
            {
                String = "Some string"
            };

            var self = new Self
            {
                Baz = baz,
                Int = 7
            };

            baz.Self = self;

            await selfApi.Operation(baz);
            Assert.True(true);
        }

        [Fact]
        public async Task AttachmentApiTests()
        {
            var api = Resolver.GetService<IAttachmentApi>();
            await api.PutSubtitleAttachmentList(new SubtitleAttachmentDto {
                SubtitleAttachmentList = new List<SubtitleAttachment>
                {
                    new SubtitleAttachment
                    {
                        Attachment = new Attachment
                        {
                            Name = "Some attachment",
                            Content = "Lorem ipsum dolar sit amet"
                        },
                        CategoryId = 2,
                        SeasonId = 1
                    }
                }
            });
            Assert.True(true);
        }
    }
}
