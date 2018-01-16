using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class ResolvedContentResult
    {
        public Dictionary<string, string> Dictionary { get; }

        public Dictionary<string, IFormFile> Files { get; }

        public ResolvedContentResult(Dictionary<string, string> dictionary, Dictionary<string, IFormFile> files = null)
        {
            Dictionary = dictionary;
            Files = files;
        }
    }
}
