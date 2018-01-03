using Microsoft.AspNetCore.Http;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public class SomeFormFileViewModel
    {
        public string Name { get; set; }
        public IFormFile Files { get; set; }
    }
}