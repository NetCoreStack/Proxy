using NetCoreStack.Contracts;
using System;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class ProxyBuilderOptions
    {
        internal List<Type> ProxyList { get; }

        public IDictionary<string, string> DefaultHeaders { get; }

        public ProxyBuilderOptions()
        {
            DefaultHeaders = new Dictionary<string, string>();
            ProxyList = new List<Type>();
        }

        public void Register<TProxy>() where TProxy : IApiContract
        {
            ProxyList.Add(typeof(TProxy));   
        }
    }
}
