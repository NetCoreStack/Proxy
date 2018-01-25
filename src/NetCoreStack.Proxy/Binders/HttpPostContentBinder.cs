using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class HttpPostContentBinder : BodyContentBinder
    {
        public HttpPostContentBinder(HttpMethod httpMethod, IModelSerializer modelSerializer)
            :base(httpMethod, modelSerializer)
        {

        }
        
        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            base.BindContent(bindingContext);
        }
    }
}