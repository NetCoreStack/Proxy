using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public class FileProxyUploadContext
    {
        public string Directory { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
