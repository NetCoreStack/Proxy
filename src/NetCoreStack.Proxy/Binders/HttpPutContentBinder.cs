using NetCoreStack.Proxy.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class HttpPutContentBinder : BodyContentBinder
    {
        HttpMethod HttpMethod => HttpMethod.Put;

        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            var isMultiPartFormData = bindingContext.IsMultiPartFormData;
            var templateKeys = bindingContext.UriDefinition.TemplateKeys;

            // bindingContext.Parameters.Select(p => p.PropertyName);

            //if (bindingContext.ArgsLength == 1)
            //{

            //}

            ModelDictionaryResult result = bindingContext.GetResolvedContentResult();
            List<string> keys = result.Dictionary.Keys.ToList();
            EnsureTemplate(bindingContext.MethodMarkerTemplate, bindingContext.UriDefinition, result.Dictionary, keys);
            if (isMultiPartFormData)
            { 
                var content = GetMultipartFormDataContent(result);
                bindingContext.ContentResult = ContentModelBindingResult.Success(content);
                return;
            }

            // PUT Request first parameter should be Id or Key then will be removed from the result dictionary
            // The remaining parameters belong to the body
            bindingContext.ContentResult = ContentModelBindingResult.Success(SerializeToString(result.Dictionary));
        }
    }
}
