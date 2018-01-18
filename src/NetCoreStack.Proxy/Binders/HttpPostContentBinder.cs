using NetCoreStack.Proxy.Extensions;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class HttpPostContentBinder : BodyContentBinder
    {
        HttpMethod HttpMethod => HttpMethod.Post;
        
        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            var isMultiPartFormData = bindingContext.IsMultiPartFormData;
            if (isMultiPartFormData)
            {
                ResolvedContentResult result = bindingContext.GetResolvedContentResult();
                var content = GetMultipartFormDataContent(result);
                bindingContext.ContentResult = ContentModelBindingResult.Success(content);
                return;
            }

            if (bindingContext.ArgsLength == 1)
            {
                bindingContext.ContentResult = ContentModelBindingResult.Success(SerializeToString(bindingContext.Args[0]));
                return;
            }

            bindingContext.ContentResult = ContentModelBindingResult.Success(SerializeToString(bindingContext.Args));
        }
    }
}