using System;
using System.Globalization;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public class RequestDescriptor
    {
        public MethodInfo TargetMethod { get; set; }
        public Type ProxyType { get; }
        public object[] Args { get; }
        public string ClientIp { get; }
        public string UserAgent { get; }
        public string Query { get; }
        public CultureInfo Culture { get; set; }

        public RequestDescriptor(MethodInfo targetMethod, 
            Type proxyType,
            string clientIp, 
            string userAgent,
            string query,
            CultureInfo culture,
            object[] args)
        {
            TargetMethod = targetMethod;
            ProxyType = proxyType;
            ClientIp = clientIp;
            UserAgent = userAgent;
            Query = query;
            Culture = culture;
            Args = args;
        }
    }
}
