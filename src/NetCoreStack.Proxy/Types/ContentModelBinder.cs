using System.Collections.Generic;
using System.Net;

namespace NetCoreStack.Proxy
{
    public abstract class ContentModelBinder : IContentModelBinder
    {
        protected virtual void EnsureTemplate(string methodMarkerTemplate,
            object[] args,
            ProxyUriDefinition proxyUriDefinition,
            Dictionary<string, string> argsDic,
            List<string> keys)
        {
            if (!string.IsNullOrEmpty(methodMarkerTemplate))
            {
                if (proxyUriDefinition.HasParameter)
                {
                    for (int i = 0; i < proxyUriDefinition.ParameterParts.Count; i++)
                    {
                        var key = keys[i];
                        proxyUriDefinition.UriBuilder.Path += ($"/{WebUtility.UrlEncode(args[i]?.ToString())}");
                        argsDic.Remove(key);
                    }
                }
            }
        }

        public abstract void BindContent(ContentModelBindingContext bindingContext);
    }
}