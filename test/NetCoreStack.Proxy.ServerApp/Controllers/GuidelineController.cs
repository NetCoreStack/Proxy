using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreStack.Proxy.Test.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.ServerApp.Controllers
{
    [Route("api/[controller]")]
    public class GuidelineController : Controller, IGuidelineApi
    {
        private readonly ILoggerFactory _loggerFactory;

        protected ILogger Logger { get; }

        public GuidelineController(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            Logger = _loggerFactory.CreateLogger<GuidelineController>();
        }

        [HttpGet(nameof(GetPostsAsync))]
        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jsonplaceholder.typicode.com/posts"));
            var response = await Factory.Client.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<Post>>(content);
            Logger.LogDebug($"{nameof(GetPostsAsync)}, PostsCount:{items.Count}");
            return items;
        }

        [HttpGet(nameof(GetWithReferenceType))]
        public async Task GetWithReferenceType([FromQuery]SimpleModel model)
        {
            var serializedModel = JsonConvert.SerializeObject(model);
            Logger.LogDebug($"{nameof(GetWithReferenceType)}, Model: {serializedModel}");
            await Task.Delay(900);
        }

        [HttpGet(nameof(PrimitiveReturn))]
        public int PrimitiveReturn(int i, string s, long l, DateTime dt)
        {
            Logger.LogDebug($"{nameof(PrimitiveReturn)}, i:{i}, s:{s}, l:{l}, dt:{dt}");
            return i + 10;
        }

        [HttpPost(nameof(TaskActionPost))]
        public async Task TaskActionPost([FromBody]SimpleModel model)
        {
            var serializedModel = JsonConvert.SerializeObject(model);
            Logger.LogDebug($"{nameof(TaskActionPost)}, Model: {serializedModel}");
            await Task.Delay(900);
        }

        [HttpGet(nameof(TaskOperation))]
        public async Task TaskOperation()
        {
            await Task.Delay(2000);
            Logger.LogDebug($"{nameof(TaskOperation)}, long running process completed!");
        }

        [HttpGet(nameof(VoidOperation))]
        public void VoidOperation()
        {
            var str = "Hello World!";
            Logger.LogDebug($"{nameof(VoidOperation)}, {str}");
        }
    }
}
