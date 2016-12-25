using NetCoreStack.Common;
using NetCoreStack.Proxy.Extensions;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Internal
{
    public static class ProxyResultExecutor
    {
        public static async Task<ResponseContext> ExecuteAsync(HttpResponseMessage response,
            RequestDescriptor descriptor,
            Type genericReturnType = null)
        {
            string metadata = response.GetMetadataHeader();
            string metadataInner = response.GetMetadataInnerHeader();
            var context = new ResponseContext(response, descriptor, metadata, metadataInner);
            var methodDescriptor = descriptor.MethodDescriptor;

            if (response == null)
                return context;

            if (typeof(ProxyException).FullName != metadata && methodDescriptor.IsCollectionResult)
            {
                if (string.IsNullOrWhiteSpace(metadataInner))
                    throw new InvalidOperationException($"{nameof(CollectionResult)} should have MetaDataInnerType info on header!");

                context.ResultContent = await response.Content.ReadAsStringAsync();

                // Direct stream transports
                if (methodDescriptor.IsDirectStreamTransport)
                {
                    context.Value = TaskHelper.CreateFromResult(methodDescriptor.ReturnType); 
                    return context;
                }

                return context;
            }

            context.ResultContent = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode >= 500)
            {
                throw new ProxyException("Proxy call result content is can not be null, " +
                                   "The exception may have occurred on the side of the API or Server", null);
            }

            if (methodDescriptor.IsVoidReturn)
                return context;

            if (methodDescriptor.IsTaskReturn)
            {
                context.Value = Task.CompletedTask;
                return context;
            }

            if (genericReturnType != null)
                context.Value = JsonConvert.DeserializeObject(context.ResultContent, genericReturnType);
            else
                context.Value = JsonConvert.DeserializeObject(context.ResultContent, methodDescriptor.ReturnType);

            return context;
        }
    }
}
