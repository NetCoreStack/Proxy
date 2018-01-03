using NetCoreStack.Contracts;
using System;
using System.Collections.Generic;

namespace NetCoreStack.Proxy
{
    public class ProxyBuilderOptions
    {
        internal List<Type> ProxyList { get; }

        internal Type ProxyContextFilter { get; set; }

        public IDictionary<string, string> DefaultHeaders { get; }

        public ProxyBuilderOptions()
        {
            DefaultHeaders = new Dictionary<string, string>();
            ProxyList = new List<Type>();
        }

        /// <summary>
        /// Register proxy interface
        /// </summary>
        /// <typeparam name="TProxy"></typeparam>
        public void Register<TProxy>() where TProxy : IApiContract
        {
            ProxyList.Add(typeof(TProxy));
        }

        /// <summary>
        /// Use the TAccessor as <see cref="IProxyContextFilter"/> instead default accessors values <see cref="DefaultProxyContextFilter"/>
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        public void RegisterFilter<TFilter>() where TFilter : IProxyContextFilter
        {
            ProxyContextFilter = typeof(TFilter);
        }
    }
}
