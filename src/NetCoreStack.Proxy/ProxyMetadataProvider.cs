using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public class ProxyMetadataProvider
    {
        private readonly TypeCache _typeCache = new TypeCache();
        private readonly Func<ProxyModelMetadataIdentity, ProxyModelMetadataCacheEntry> _cacheEntryFactory;
        private readonly ProxyModelMetadataCacheEntry _metadataCacheEntryForObjectType;

        public ProxyMetadataProvider()
        {
            _cacheEntryFactory = CreateCacheEntry;
            _metadataCacheEntryForObjectType = GetMetadataCacheEntryForObjectType();
        }

        private ProxyModelMetadataCacheEntry GetMetadataCacheEntryForObjectType()
        {
            var key = ProxyModelMetadataIdentity.ForType(typeof(object));
            var entry = CreateCacheEntry(key);
            return entry;
        }

        private ProxyModelMetadataCacheEntry CreateCacheEntry(ProxyModelMetadataIdentity key)
        {
            var metadata = new ProxyModelMetadata(key);
            return new ProxyModelMetadataCacheEntry(metadata);
        }

        private ProxyModelMetadataCacheEntry GetCacheEntry(Type modelType)
        {
            ProxyModelMetadataCacheEntry cacheEntry;

            if (modelType == typeof(object))
            {
                cacheEntry = _metadataCacheEntryForObjectType;
            }
            else
            {
                var key = ProxyModelMetadataIdentity.ForType(modelType);

                cacheEntry = _typeCache.GetOrAdd(key, _cacheEntryFactory);
            }

            return cacheEntry;
        }

        private ProxyModelMetadataCacheEntry GetCacheEntry(ParameterInfo parameter)
        {
            return _typeCache.GetOrAdd(
                ProxyModelMetadataIdentity.ForParameter(parameter),
                _cacheEntryFactory);
        }

        public ProxyModelMetadata GetMetadataForParameter(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var cacheEntry = GetCacheEntry(parameter);

            return cacheEntry.Metadata;
        }

        public ProxyModelMetadata GetMetadataForType(Type modelType)
        {
            if (modelType == null)
            {
                throw new ArgumentNullException(nameof(modelType));
            }

            var cacheEntry = GetCacheEntry(modelType);

            return cacheEntry.Metadata;
        }

        private class TypeCache : ConcurrentDictionary<ProxyModelMetadataIdentity, ProxyModelMetadataCacheEntry>
        {
        }

        private struct ProxyModelMetadataCacheEntry
        {
            public ProxyModelMetadataCacheEntry(ProxyModelMetadata metadata)
            {
                Metadata = metadata;
            }

            public ProxyModelMetadata Metadata { get; }
        }
    }
}
