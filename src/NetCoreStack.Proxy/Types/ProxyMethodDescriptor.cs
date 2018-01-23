using Microsoft.AspNetCore.Routing.Template;
using NetCoreStack.Proxy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public class ProxyMethodDescriptor
    {
        public string MethodMarkerTemplate { get; set; }

        public MethodInfo MethodInfo { get; }

        public Type ReturnType { get; }

        public Type UnderlyingReturnType { get; }

        public HttpMethod HttpMethod { get; set; }

        public TimeSpan? Timeout { get; set; }
        public RouteTemplate RouteTemplate { get; set; }

        public bool IsMultiPartFormData { get; set; }

        public List<ProxyModelMetadata> Parameters { get; }

        public bool IsVoidReturn { get; }

        public bool IsTaskReturn { get; }

        public bool IsGenericTaskReturn { get; }

        public List<string> TemplateKeys { get; set; }

        public List<string> TemplateParameterKeys { get; set; }

        public List<TemplatePart> TemplateParts { get; set; }

        public bool HasAnyTemplateParameterKey => TemplateParameterKeys.Any();

        public Dictionary<string, string> Headers { get; }

        public ProxyMethodDescriptor(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            ReturnType = methodInfo.ReturnType;
            IsVoidReturn = ReturnType == typeof(void);
            IsTaskReturn = ReturnType.IsAssignableFrom(typeof(Task)) ? true : false;
            IsGenericTaskReturn = ReturnType.IsGenericTask() ? true : false;
            Headers = new Dictionary<string, string>(StringComparer.Ordinal);
            Parameters = new List<ProxyModelMetadata>();
            TemplateParts = new List<TemplatePart>();
            TemplateKeys = new List<string>();
            TemplateParameterKeys = new List<string>();

            if (IsGenericTaskReturn)
            {
                UnderlyingReturnType = ReturnType.GetGenericArguments()[0];
            }
        }
    }
}
