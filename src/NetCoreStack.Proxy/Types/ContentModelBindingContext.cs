using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool HasAnyTemplateParameterKey
        {
            get
            {
                return TemplateParameterKeys.Any();
            }
        }

        public UriBuilder UriBuilder => UriDefinition.UriBuilder;
        public Uri Uri => UriBuilder.Uri;
        public string MethodMarkerTemplate => MethodDescriptor.MethodMarkerTemplate;
        public List<TemplatePart> TemplateParts => MethodDescriptor.TemplateParts;
        public List<string> TemplateParameterKeys => MethodDescriptor.TemplateParameterKeys;
        public List<ProxyModelMetadata> Parameters => MethodDescriptor.Parameters;
        public bool IsMultiPartFormData { get; set; }
        public ContentModelBindingResult ContentResult { get; set; }
        public object[] Args { get; set; }
        public IModelContentResolver ModelContentResolver { get; set; }
        public HttpMethod HttpMethod { get; }
        public ProxyUriDefinition UriDefinition { get; }
        public ProxyMethodDescriptor MethodDescriptor { get; }

        public ContentModelBindingContext(HttpMethod httpMethod, ProxyMethodDescriptor methodDescriptor, ProxyUriDefinition proxyUriDefinition)
        {
            HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            MethodDescriptor = methodDescriptor ?? throw new ArgumentNullException(nameof(methodDescriptor));
            UriDefinition = proxyUriDefinition ?? throw new ArgumentNullException(nameof(proxyUriDefinition));
        }
    }
}