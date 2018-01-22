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
            var hasAnyTemplateParameterKey = bindingContext.HasAnyTemplateParameterKey;
            if (bindingContext.ArgsLength == 1 && hasAnyTemplateParameterKey)
            {
                ModelDictionaryResult mdr = bindingContext.ModelContentResolver.Resolve(bindingContext.Parameters, bindingContext.Args);
                List<string> mdrKeys = mdr.Dictionary.Keys.ToList();
                EnsureTemplate(bindingContext, mdr.Dictionary, mdrKeys);
                return;
            }

            //List<ProxyModelMetadata> modelMetadataKeyList = new List<ProxyModelMetadata>();
            //int parameterOffset = 0;
            //foreach (var key in templateParameterKeys)
            //{
            //    // Key template must be top level object property or parameter
            //    ProxyModelMetadata keyModelMetadata = bindingContext.Parameters.FirstOrDefault(p => p.PropertyName == key);
            //    if (keyModelMetadata != null)
            //    {
            //        modelMetadataKeyList.Add(keyModelMetadata);
            //        bindingContext.Parameters.Remove(keyModelMetadata);
            //        parameterOffset++;
            //    }
            //}


            ModelDictionaryResult result = bindingContext.ModelContentResolver.Resolve(bindingContext.Parameters, bindingContext.Args);
            List<string> keys = result.Dictionary.Keys.ToList();
            EnsureTemplate(bindingContext, result.Dictionary, keys);

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
