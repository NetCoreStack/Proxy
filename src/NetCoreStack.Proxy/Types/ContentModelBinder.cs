using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NetCoreStack.Proxy
{
    public abstract class ContentModelBinder : IContentModelBinder
    {
        protected virtual void EnsureTemplate(ContentModelBindingContext context, Dictionary<string, string> argsDic, List<string> keys)
        {
            if (!string.IsNullOrEmpty(context.MethodMarkerTemplate))
            {
                for (int i = 0; i < context.ParameterParts.Count; i++)
                {
                    var key = keys[i];
                    if (argsDic.TryGetValue(key, out string value))
                    {
                        context.UriBuilder.Path += ($"/{WebUtility.UrlEncode(value)}");
                        argsDic.Remove(key);
                    }
                }
            }
        }

        public abstract void BindContent(ContentModelBindingContext bindingContext);
    }
}