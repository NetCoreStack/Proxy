using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public class RequestContext
    {
        public MethodInfo TargetMethod { get; set; }
        public Type ProxyType { get; }
        public object[] Args { get; }
        public string ClientIp { get; }
        public string UserAgent { get; }
        public string QueryString { get; }
        public KeyValuePair<string, string> TokenCookie { get; }

        public RequestContext(MethodInfo targetMethod, Type proxyType,
            string clientIp, string userAgent,
            KeyValuePair<string, string> tokenCookie,
            string queryString, object[] args)
        {
            TargetMethod = targetMethod;
            ProxyType = proxyType;
            ClientIp = clientIp;
            UserAgent = userAgent;
            TokenCookie = tokenCookie;
            QueryString = queryString;
            Args = args;
        }
    }
}
