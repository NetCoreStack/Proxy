using NetCoreStack.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NetCoreStack.Proxy
{
    public class ProxyBuilderOptions
    {
        internal List<Type> ProxyList { get; }

        internal Type ProxyContextFilter { get; set; }

        public IDictionary<string, string> DefaultHeaders { get; }

        public IList<IModelResolver> ModelResolvers { get; }

        public Func<CultureInfo> CultureFactory { get; set; }

        public ProxyBuilderOptions()
        {
            DefaultHeaders = new Dictionary<string, string>();
            ProxyList = new List<Type>();
            ModelResolvers = new List<IModelResolver>();
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
