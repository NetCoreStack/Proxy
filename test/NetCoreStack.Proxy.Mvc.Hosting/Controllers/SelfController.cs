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
    public class SelfController : Controller, ISelfApi
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public SelfController(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _hostingEnvironment = hostingEnvironment;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<SelfController>();
        }

        [HttpPost(nameof(Operation))]
        public Task Operation([FromBody]Baz model)
        {
            _logger.LogWarning(JsonConvert.SerializeObject(model));
            return Task.CompletedTask;
        }

        [HttpPost(nameof(OperationCollection))]
        public Task OperationCollection([FromBody]Baz2 model)
        {
            _logger.LogWarning(JsonConvert.SerializeObject(model));
            return Task.CompletedTask;
        }
    }
}