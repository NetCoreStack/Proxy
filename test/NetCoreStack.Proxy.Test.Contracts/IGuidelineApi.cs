using NetCoreStack.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [ApiRoute("api/[controller]", regionKey: "Main")]
    public interface IGuidelineApi : IApiContract
    {
        Task VoidOperation();

        Task<int> PrimitiveReturn(int i, string s, long l, DateTime dt);

        Task TaskOperation();

        Task<IEnumerable<SampleModel>> GetEnumerableModels();

        Task<CollectionResult<SampleModel>> GetCollectionStreamTask();

        IEnumerable<CollectionResult<SampleModel>> GetCollection();

        Task GetWithReferenceType(SampleModel model);

        [HttpPostMarker]
        Task TaskActionPost(SampleModel model);

        [HttpPutMarker]
        Task TaskActionPut(long id, SampleModel model);

        [HttpDeleteMarker]
        Task TaskActionDelete(long id);
    }
}
