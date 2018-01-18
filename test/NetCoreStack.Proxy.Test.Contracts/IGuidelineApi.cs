using NetCoreStack.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [ApiRoute("api/[controller]", regionKey: "Main")]
    public interface IGuidelineApi : IApiContract
    {
        Task TaskOperation();

        Task GetComplexType(ComplexTypeModel model);

        Task<int> PrimitiveReturn(int i, string s, long l, DateTime dt);

        Task GetWithComplexReferenceType(CollectionRequest request);

        Task<IEnumerable<SampleModel>> GetEnumerableModels();

        Task<CollectionResult<SampleModel>> GetCollectionStreamTask();

        [HttpPostMarker]
        Task TaskActionPost(ComplexTypeModel model);

        [HttpPutMarker]
        Task TaskActionPut(long id, SampleModel model);

        [HttpDeleteMarker]
        Task TaskActionDelete(long id);
    }
}
