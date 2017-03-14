using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.WebClient.Controllers
{
    public class TestController : Controller
    {
        private readonly IGuidelineApi _api;

        public TestController(IGuidelineApi api)
        {
            _api = api;
        }

        public async Task<IActionResult> GetPostsAsync()
        {
            var items = await _api.GetPostsAsync();
            return Json(items);
        }

        public async Task<IActionResult> DirectStreamTransport()
        {
            var items = await _api.GetCollectionStream();
            return Json(items);
        }

        public IActionResult DirectStreamTransports()
        {
            var items = _api.GetCollectionStreams();
            return Json(items);
        }

        public async Task<IActionResult> GetWithReferenceType()
        {
            var simpleModel = new SimpleModel
            {
                Name = "WebClient",
                Date = DateTime.Now,
                Value = "<<string>>"
            };

            await _api.GetWithReferenceType(simpleModel);
            return Json(simpleModel);
        }
    }
}
