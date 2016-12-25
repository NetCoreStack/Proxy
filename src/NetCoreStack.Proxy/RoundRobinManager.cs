using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreStack.Proxy
{
    public class RoundRobinManager
    {
        private static readonly object _lockObj = new object();

        public readonly ConcurrentDictionary<string, Queue<string>> ProxyRegionDict =
            new ConcurrentDictionary<string, Queue<string>>();

        protected ProxyOptions Options { get; }

        public RoundRobinManager(IOptions<ProxyOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Value == null || options.Value.RegionKeys == null)
            {
                throw new ArgumentNullException(nameof(options.Value.RegionKeys));
            }

            Options = options.Value;

            Dictionary<string, string> regionKeys = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> entry in Options.RegionKeys)
            {
                if (string.IsNullOrEmpty(entry.Value))
                    continue;

                var urls = entry.Value.Split(',').Select(p => p.Trim()).ToList();
                ProxyRegionDict.TryAdd(entry.Key, new Queue<string>(urls));
            }
        }

        public UriBuilder RoundRobinUri(string regionKey)
        {
            UriBuilder uri = null;
            Queue<string> queue;
            if (!ProxyRegionDict.TryGetValue(regionKey, out queue))
            {
                throw new ArgumentOutOfRangeException($"Region could not be found! {nameof(RoundRobinManager)}: \"{regionKey}\"");
            }

            lock (_lockObj)
            {
                var url = queue.Dequeue();
                uri = new UriBuilder(url);
                queue.Enqueue(url);
                return uri;
            }
        }
    }
}
