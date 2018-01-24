using NetCoreStack.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    [ApiRoute("api/[controller]", regionKey: "Main")]
    public interface IGuidelineApi : IApiContract
    {
        [HttpHeaders("X-Method-Header: Some Value")]
        Task TaskOperation();

        Task GetComplexType(ComplexTypeModel model);

        Task<int> PrimitiveReturn(int i, string s, long l, DateTime dt);

        Task GetWithComplexReferenceType(CollectionRequest request);

        Task<IEnumerable<SampleModel>> GetEnumerableModels();

        Task<CollectionResult<SampleModel>> GetCollectionStreamTask();

        /// <summary>
        /// Default Content-Type is ModelAware if it is not contain FormFile type property that will be JSON body
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPostMarker]
        Task TaskActionPost(ComplexTypeModel model);

        [HttpPostMarker(ContentType = ContentType.MultipartFormData)]
        Task TaskActionBarMultipartFormData(Bar model);

        [HttpPostMarker(ContentType = ContentType.Xml)]
        Task TaskActionBarSimpleXml(BarSimple model);

        [HttpPostMarker]
        Task TaskSingleFileModel(SingleFileModel model);

        [HttpPostMarker]
        Task TaskEnumerableFileModel(EnumerableFileModel model);

        [HttpPutMarker(Template = "TaskKeyAndSingleFileModel/{key}")]
        Task TaskKeyAndSingleFileModel(string key, SingleFileModel model);

        [HttpPutMarker(Template = "TaskKeyAndMultipleFileModel/{key}")]
        Task TaskKeyAndEnumerableFileModel(string key, EnumerableFileModel model);

        [HttpPutMarker(Template = "TaskKeyAndSingleFileAndPropsModel/{key}")]
        Task TaskKeyAndSingleFileAndPropsModel(string key, SingleFileAndPropsModel model);

        [HttpPutMarker(Template = "TaskKeySingleFileAndBarModel/{key}")]
        Task TaskKeySingleFileAndBarModel(string key, SingleFileModel model, Bar bar);

        /// <summary>
        /// Template and parameter usage, key parameter will be part of the request Url and extracting it as api/guideline/kv/<key>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPutMarker(Template = "kv/{key}")]
        Task<bool> CreateOrUpdateKey(string key, Bar body);

        [HttpPutMarker(Template = "kv/nobody/{key}")]
        Task<bool> CreateOrUpdateKey(string key);

        [HttpDeleteMarker]
        Task TaskActionDelete(long id);
    }
}
