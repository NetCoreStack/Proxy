using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public class ProxyDescriptor
    {
        public Type ProxyType { get; }

        public string RegionKey { get; }

        public string Route { get; }

        public IDictionary<MethodInfo, ProxyMethodDescriptor> Methods { get; set; }

        public ProxyDescriptor(Type proxyType, string regionKey, string route)
        {
            ProxyType = proxyType;
            RegionKey = regionKey;
            Route = route;
            Methods = new Dictionary<MethodInfo, ProxyMethodDescriptor>();
        }
    }
}
