using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Contracts;
using NetCoreStack.Mvc;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [NetCoreStack.Contracts.ApiRoute("api/[controller]", regionKey: "Main")]
    public interface IAttachmentApi : IApiContract
    {
        [HttpGetMarker]
        Task<ApiResult<Attachment>> GetFileMetaData([FromHeader]long attachmentId);

        [HttpPutMarker]
        Task<ApiResult<bool>> PutSubtitleAttachmentList([FromBody] SubtitleAttachmentDto attachments);
    }
}
