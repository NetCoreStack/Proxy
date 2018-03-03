using NetCoreStack.Contracts;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [ApiRoute("api/[controller]", regionKey: "Main")]
    public interface ISelfApi : IApiContract
    {
        [HttpPostMarker]
        Task Operation(Baz model);

        [HttpPostMarker]
        Task OperationCollection(Baz2 model);
    }
}