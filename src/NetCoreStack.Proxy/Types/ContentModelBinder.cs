using System.Collections.Generic;
using System.Net;

namespace NetCoreStack.Proxy
{
    public abstract class ContentModelBinder : IContentModelBinder
    {
        protected virtual void EnsureTemplate(string methodMarkerTemplate,
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
                        if(argsDic.TryGetValue(key, out string value))
                        {
                            proxyUriDefinition.UriBuilder.Path += ($"/{WebUtility.UrlEncode(value)}");
                            argsDic.Remove(key);
                        }
                    }
                }
            }
        }

        public abstract void BindContent(ContentModelBindingContext bindingContext);
    }
}