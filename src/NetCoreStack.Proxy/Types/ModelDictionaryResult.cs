using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class ModelDictionaryResult
    {
        public Dictionary<string, string> Dictionary { get; }

        public Dictionary<string, IFormFile> Files { get; }

        public ModelDictionaryResult(Dictionary<string, string> dictionary, Dictionary<string, IFormFile> files = null)
        {
            Dictionary = dictionary;
            Files = files;
        }
    }
}