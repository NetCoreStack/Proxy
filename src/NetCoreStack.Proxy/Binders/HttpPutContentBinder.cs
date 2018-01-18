using NetCoreStack.Proxy.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NetCoreStack.Contracts;

namespace NetCoreStack.Proxy
{
    public class HttpPutContentBinder : BodyContentBinder
    {
        HttpMethod HttpMethod => HttpMethod.Put;

        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            ResolvedContentResult result = bindingContext.GetResolvedContentResult();
            List<string> keys = result.Dictionary.Keys.ToList();
            EnsureTemplate(bindingContext.MethodMarkerTemplate, bindingContext.Args, bindingContext.UriDefinition, result.Dictionary, keys);
            if (bindingContext.ArgsLength == 1)
            {
                bindingContext.ContentResult = ContentModelBindingResult.Success(SerializeToString(bindingContext.Args[0]));
                return;
                // request.Content = SerializeToString(argsDic.First().Value);
            }
            else if (bindingContext.ArgsLength == 2)
            {
                var firstParameter = result.Dictionary[keys[0]];
                var secondParameter = result.Dictionary[keys[1]];

                // PUT Request first parameter should be Id or Key
                if (firstParameter.GetType().IsPrimitive())
                {
                    bindingContext.UriBuilder.Query += string.Format("&{0}={1}", keys[0], firstParameter);
                }

                // request.RequestUri = uriBuilder.Uri;
                // request.Content = SerializeToString(secondParameter);
                bindingContext.ContentResult = ContentModelBindingResult.Success(SerializeToString(secondParameter));
                return;
            }
        }
    }
}
