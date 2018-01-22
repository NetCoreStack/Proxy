using NetCoreStack.Proxy.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class HttpDeleteContentBinder : ContentModelBinder
    {
        HttpMethod HttpMethod => HttpMethod.Delete;

        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            ModelDictionaryResult result = bindingContext.ModelContentResolver.Resolve(bindingContext.Parameters, bindingContext.Args);
            List<string> keys = result.Dictionary.Keys.ToList();
            EnsureTemplate(bindingContext, result.Dictionary, keys);

            bindingContext.TryUpdateUri(result.Dictionary);
        }
    }
}