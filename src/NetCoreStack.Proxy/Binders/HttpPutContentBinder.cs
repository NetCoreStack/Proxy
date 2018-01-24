using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class HttpPutContentBinder : BodyContentBinder
    {
        public HttpPutContentBinder(HttpMethod httpMethod, IModelSerializer modelSerializer)
            : base(httpMethod, modelSerializer)
        {

        }

        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            base.BindContent(bindingContext);
        }
    }
}
