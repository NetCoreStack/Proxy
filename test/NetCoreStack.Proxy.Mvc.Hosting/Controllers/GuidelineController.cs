using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Test.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Mvc.Hosting.Controllers
{
    [Route("api/[controller]")]
    public class GuidelineController : Controller, IGuidelineApi
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public GuidelineController(ILoggerFactory loggerFactory)
        {
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

        [HttpPost(nameof(TaskActionPost))]
        public async Task TaskActionPost([FromBody]ComplexTypeModel model)
        {
            await Task.CompletedTask;
            _logger.LogWarning(JsonConvert.SerializeObject(model));
        }

        [HttpPut("kv")]
        public async Task<bool> CreateOrUpdateKey(string key, Bar body)
        {
            await Task.CompletedTask;
            _logger.LogWarning(JsonConvert.SerializeObject(new { key, body }));
            return true;
        }

        [HttpDelete(nameof(TaskActionDelete))]
        public async Task TaskActionDelete(long id)
        {
            await Task.CompletedTask;
            _logger.LogWarning(JsonConvert.SerializeObject(new { id }));
        }
    }
}
