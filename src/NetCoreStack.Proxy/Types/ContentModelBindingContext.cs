using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class ContentModelBindingContext
    {
        public int ArgsLength
        {
            get
            {
                return Args != null ? Args.Length : 0;
            }
        }

        public UriBuilder UriBuilder => UriDefinition?.UriBuilder;
        public Uri Uri => UriBuilder.Uri;
        public string MethodMarkerTemplate { get; set; }
        public bool IsMultiPartFormData { get; set; }
        public object[] Args { get; set; }
        public IModelContentResolver ModelContentResolver { get; set; }
        public List<ProxyModelMetadata> Parameters { get; set; }
        public ContentModelBindingResult ContentResult { get; set; }
        public ProxyUriDefinition UriDefinition { get; set; }
        public HttpMethod HttpMethod { get; set; }
    }
}