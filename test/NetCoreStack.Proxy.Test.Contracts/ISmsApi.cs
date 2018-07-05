using NetCoreStack.Contracts;
using NetCoreStack.Mvc;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [ApiRoute("api/[controller]", "Integrations")]
    public interface ISmsApi : IApiContract
    {
        Task<ApiResult<SmsResult>> Send(string telephone, string content, bool isOtp = false, int? minutesTimeout = null);
    }

    public class SmsResult
    {
        public bool Successfull { get; set; }
        public string MessageId { get; set; }
    }
}
