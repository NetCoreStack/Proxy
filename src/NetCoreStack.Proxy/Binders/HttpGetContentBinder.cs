using NetCoreStack.Proxy.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class HttpGetContentBinder : ContentModelBinder
    {
        HttpMethod HttpMethod => HttpMethod.Get;

        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            ModelDictionaryResult result = bindingContext.GetResolvedContentResult();
            List<string> keys = result.Dictionary.Keys.ToList();
            EnsureTemplate(bindingContext.MethodMarkerTemplate, bindingContext.Args, bindingContext.UriDefinition, result.Dictionary, keys);

            bindingContext.TryUpdateUri(result.Dictionary);
        }
    }
}
