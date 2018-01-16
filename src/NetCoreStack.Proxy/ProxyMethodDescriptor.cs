using NetCoreStack.Proxy.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public class ProxyMethodDescriptor
    {
        public string Template { get; set; }

        public MethodInfo MethodInfo { get; }

        public Type ReturnType { get; }

        public Type UnderlyingReturnType { get; }

        public HttpMethod HttpMethod { get; set; }

        public TimeSpan? Timeout { get; set; }

        public bool IsMultiPartFormData { get; set; }

        public List<ProxyModelMetadata> Parameters { get; set; }

        public bool IsVoidReturn { get; }

        public bool IsTaskReturn { get; }

        public bool IsGenericTaskReturn { get; }

        public ProxyMethodDescriptor(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            ReturnType = methodInfo.ReturnType;
            IsVoidReturn = ReturnType == typeof(void);
            IsTaskReturn = ReturnType.IsAssignableFrom(typeof(Task)) ? true : false;
            IsGenericTaskReturn = ReturnType.IsGenericTask() ? true : false;

            if (IsGenericTaskReturn)
            {
                UnderlyingReturnType = ReturnType.GetGenericArguments()[0];
            }
        }
    }
}
