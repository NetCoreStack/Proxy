using NetCoreStack.Contracts;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [ApiRoute("/", regionKey: "Main")]
    public interface IFileProxyApi : IApiContract
    {
        Task GetFilesAsync(params string[] fileNames);

        [HttpPostMarker(Template = "upload")]
        Task UploadAsync(FileProxyUploadContext context);
    }
}