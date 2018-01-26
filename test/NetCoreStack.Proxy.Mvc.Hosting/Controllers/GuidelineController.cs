using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Test.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Mvc.Hosting.Controllers
{
    [Route("api/[controller]")]
    public class GuidelineController : Controller, IGuidelineApi
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public GuidelineController(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _hostingEnvironment = hostingEnvironment;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<GuidelineController>();
        }

        [HttpGet(nameof(GetComplexType))]
        public async Task GetComplexType(ComplexTypeModel model)
        {
            await Task.CompletedTask;
            var query = HttpContext.Request.QueryString.HasValue ? HttpContext.Request.QueryString.Value : string.Empty;
            _logger.LogWarning(JsonConvert.SerializeObject(model));
        }

        [HttpGet(nameof(TaskOperation))]
        public async Task TaskOperation()
        {
            await Task.CompletedTask;
        }

        [HttpGet(nameof(PrimitiveReturn))]
        public async Task<int> PrimitiveReturn(int i, string s, long l, DateTime dt)
        {
            await Task.CompletedTask;
            var query = HttpContext.Request.QueryString.HasValue ? HttpContext.Request.QueryString.Value : string.Empty;
            _logger.LogWarning(JsonConvert.SerializeObject(new { i, s, l, dt, query }));
            return 17;
        }

        [HttpGet(nameof(GetWithComplexReferenceType))]
        public async Task GetWithComplexReferenceType(CollectionRequest request)
        {
            await Task.CompletedTask;
            var query = HttpContext.Request.QueryString.HasValue ? HttpContext.Request.QueryString.Value : string.Empty;
            _logger.LogWarning(JsonConvert.SerializeObject(new { request, query }));
        }

        [HttpGet(nameof(GetEnumerableModels))]
        public Task<IEnumerable<SampleModel>> GetEnumerableModels()
        {
            // HttpLive
            return null;
        }

        [HttpGet(nameof(GetCollectionStreamTask))]
        public Task<CollectionResult<SampleModel>> GetCollectionStreamTask()
        {
            // HttpLive
            return null;
        }

        [HttpPost(nameof(TaskComplexTypeModel))]
        public async Task TaskComplexTypeModel([FromBody]ComplexTypeModel model)
        {
            await Task.CompletedTask;
            _logger.LogWarning(JsonConvert.SerializeObject(model));
        }

        [HttpPost(nameof(TaskSingleFileModel))]
        public async Task TaskSingleFileModel(SingleFileModel model)
        {
            await Task.CompletedTask;

            var name = model.File.Name;
            var fileName = model.File.FileName;
            var length = model.File.Length;

            using (var ms = new MemoryStream())
            {
                model.File.CopyTo(ms);
                System.IO.File.WriteAllBytes(Path.Combine(_hostingEnvironment.ContentRootPath, fileName), ms.ToArray());
            }

            _logger.LogWarning(JsonConvert.SerializeObject(new { name, fileName, length }));
        }

        [HttpPost(nameof(TaskActionBarMultipartFormData))]
        public Task TaskActionBarMultipartFormData(Bar model)
        {
            throw new NotImplementedException();
        }

        [HttpPut("kv")]
        public async Task<bool> CreateOrUpdateKey(string key, Bar body)
        {
            await Task.CompletedTask;
            _logger.LogWarning(JsonConvert.SerializeObject(new { key, body }));
            return true;
        }

        [HttpPut("kv/nobody")]
        public Task<bool> CreateOrUpdateKey(string key)
        {
            throw new NotImplementedException();
        }

        [HttpPut(nameof(TaskKeyAndSingleFileModel))]
        public Task TaskKeyAndSingleFileModel(string key, SingleFileModel model)
        {
            throw new NotImplementedException();
        }

        [HttpPut(nameof(TaskKeySingleFileAndBarModel))]
        public Task TaskKeySingleFileAndBarModel(string key, SingleFileModel model, Bar bar)
        {
            throw new NotImplementedException();
        }

        [HttpDelete(nameof(TaskActionDelete))]
        public async Task TaskActionDelete(long id)
        {
            await Task.CompletedTask;
            _logger.LogWarning(JsonConvert.SerializeObject(new { id }));
        }

        public Task TaskKeyAndSingleFileAndPropsModel(string key, SingleFileAndPropsModel model)
        {
            throw new NotImplementedException();
        }

        public Task TaskEnumerableFileModel(EnumerableFileModel model)
        {
            throw new NotImplementedException();
        }

        public Task TaskKeyAndEnumerableFileModel(string key, EnumerableFileModel model)
        {
            throw new NotImplementedException();
        }

        public Task TaskActionBarSimpleXml(BarSimple model)
        {
            throw new NotImplementedException();
        }

        public Task UploadAsync(FileProxyUploadContext context)
        {
            throw new NotImplementedException();
        }
    }
}