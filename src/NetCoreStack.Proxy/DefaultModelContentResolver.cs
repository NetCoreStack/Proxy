using NetCoreStack.Proxy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public class DefaultModelContentResolver : IModelContentResolver
    {
        public ProxyMetadataProvider MetadataProvider { get; }

        public DefaultModelContentResolver(ProxyMetadataProvider metadataProvider)
        {
            MetadataProvider = metadataProvider;
        }

        // Per request parameter context resolver
        public ResolvedContentResult Resolve(List<ProxyModelMetadata> parameters, 
            HttpMethod httpMethod, 
            bool isMultiPartFormData,
            object[] args)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
            if (httpMethod == HttpMethod.Get)
            {
                // Ref type parameter resolver
                if (parameters.Count == 1 && parameters[0].IsReferenceType)
                {
                    var modelMetadata = MetadataProvider.GetMetadataForType(args[0].GetType());

                    var obj = args[0].ToDictionary();

                    // TODO Gencebay
                    // values.Merge(obj, true);
                    // return values;
                }

                if (parameters.Count > 1 && parameters.Any(x => x.IsReferenceType))
                {
                    throw new ArgumentOutOfRangeException($"Methods marked with HTTP GET can take only one reference type parameter at the same time.");
                }
            }

            // TODO Gencebay
            // values.MergeArgs(args, parameters, isMultiPartFormData);
            // return values;

            return new ResolvedContentResult(dictionary);
        }
    }
}
