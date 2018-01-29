using NetCoreStack.Contracts;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [ApiRoute("/", regionKey: "Main")]
    public interface IFileProxyApi : IApiContract
    {
        [HttpGetMarker(Template = "proxy-fs/{id}/{filename}")]
        Task<byte[]> GetFileAsync(string id, string filename);

        [HttpPostMarker(Template = "upload")]
        Task UploadAsync(FileProxyUploadContext context);
    }
}