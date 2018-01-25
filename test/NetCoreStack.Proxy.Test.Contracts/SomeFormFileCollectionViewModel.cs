using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public class SomeFormFileCollectionViewModel
    {
        public string Name { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
    }
}