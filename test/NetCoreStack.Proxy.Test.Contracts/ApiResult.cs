using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace NetCoreStack.Proxy.Tests.Types
{
    public class ApiResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.FromResult(0);
        }
    }

    public sealed class ApiResult<T> : ApiResult
    {
        public T Result { get; set; }

        public ApiResult(T result = default(T), string message = "", int statusCode = StatusCodes.Status200OK)
        {
            Result = result;
        }
    }
}
