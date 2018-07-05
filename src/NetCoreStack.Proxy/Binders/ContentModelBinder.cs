using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public abstract class ContentModelBinder : IContentModelBinder
    {
        protected HttpMethod HttpMethod { get; }

        public ContentModelBinder(HttpMethod httpMethod)
        {
            HttpMethod = httpMethod;
        }

        protected virtual EnsureTemplateResult EnsureTemplate(ContentModelBindingContext bindingContext)
        {
            int parameterOffset = 0;
            if (bindingContext.HasAnyTemplateParameterKey)
            {
                for (int i = 0; i < bindingContext.TemplateParameterKeys.Count; i++)
                {
                    var keyParameter = bindingContext.TemplateParameterKeys[i];
                    var keyModelMetadata = bindingContext.Parameters.FirstOrDefault(x => x.PropertyName == keyParameter);
                    if (keyModelMetadata == null)
                    {
                        throw new ArgumentOutOfRangeException("Key parameter name does not match the template key. Please check template key(s) of the method.");
                    }

                    var value = bindingContext.ModelContentResolver.ResolveParameter(keyModelMetadata, bindingContext.Args[i], false);
                    bindingContext.UriBuilder.Path += ($"/{Uri.EscapeUriString(value)}");
                    parameterOffset++;
                }
            }

            bool ignorePrefix = false;
            if (parameterOffset == bindingContext.ArgsLength - 1)
            {
                ignorePrefix = true;
            }

            if (parameterOffset == bindingContext.ArgsLength)
            {
                // all parameters are resolved
                return EnsureTemplateResult.Completed(parameterOffset, ignorePrefix);
            }

            return EnsureTemplateResult.Continue(parameterOffset, ignorePrefix);
        }

        public abstract void BindContent(ContentModelBindingContext bindingContext);
    }
}