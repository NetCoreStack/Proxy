using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public class SomeFormComplexViewModel
    {
        public string Name { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public ICollection<string> SomeCollection { get; set; }

        public IFormFile Files { get; set; }
    }
}