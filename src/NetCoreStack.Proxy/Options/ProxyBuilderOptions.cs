using NetCoreStack.Contracts;
using System;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class ProxyBuilderOptions
    {
        internal List<Type> ProxyList { get; }

        public ProxyBuilderOptions()
        {
            ProxyList = new List<Type>();
        }

        public void Register<TProxy>() where TProxy : IApiContract
        {
            ProxyList.Add(typeof(TProxy));   
        }
    }
}
