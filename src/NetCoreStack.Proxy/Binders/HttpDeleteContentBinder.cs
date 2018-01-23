using NetCoreStack.Proxy.Extensions;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class HttpDeleteContentBinder : ContentModelBinder
    {
        HttpMethod HttpMethod => HttpMethod.Delete;

        public override void BindContent(ContentModelBindingContext bindingContext)
        {
            EnsureTemplateResult ensureTemplateResult = EnsureTemplate(bindingContext);
            if (ensureTemplateResult.BindingCompleted)
                return;

            ModelDictionaryResult result = bindingContext.ModelContentResolver.Resolve(bindingContext.Parameters, 
                bindingContext.Args, 
                ensureTemplateResult.ParameterOffset);

            bindingContext.TryUpdateUri(result.Dictionary);
        }
    }
}