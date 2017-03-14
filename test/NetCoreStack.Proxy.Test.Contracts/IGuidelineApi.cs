using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [ApiRoute("api/[controller]", regionKey: "Main")]
    public interface IGuidelineApi : IApiContract
    {
        void VoidOperation();

        int PrimitiveReturn(int i, string s, long l, DateTime dt);

        Task TaskOperation();

        Task<IEnumerable<Post>> GetPostsAsync();

        Task<CollectionResult<Post>> GetCollectionStream();

        IEnumerable<CollectionResult<Post>> GetCollectionStreams();

        Task GetWithReferenceType(SimpleModel model);

        [HttpPost]
        Task TaskActionPost(SimpleModel model);
    }
}
