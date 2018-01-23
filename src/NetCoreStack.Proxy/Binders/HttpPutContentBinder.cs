using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class HttpPutContentBinder : BodyContentBinder
    {
        HttpMethod HttpMethod => HttpMethod.Put;

        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            EnsureTemplateResult ensureTemplateResult = EnsureTemplate(bindingContext);
            if (ensureTemplateResult.BindingCompleted)
                return;

            ModelDictionaryResult result = bindingContext.ModelContentResolver.Resolve(bindingContext.Parameters,
                bindingContext.Args,
                ensureTemplateResult.ParameterOffset,
                ensureTemplateResult.IgnoreModelPrefix);

            var isMultiPartFormData = bindingContext.IsMultiPartFormData;
            if (isMultiPartFormData)
            { 
                var content = GetMultipartFormDataContent(result);
                bindingContext.ContentResult = ContentModelBindingResult.Success(content);
                return;
            }

            bindingContext.ContentResult = ContentModelBindingResult.Success(SerializeToString(result.Dictionary));
        }
    }
}
