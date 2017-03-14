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
            var context = new ResponseContext(response, descriptor);
            var methodDescriptor = descriptor.MethodDescriptor;

            if (response == null)
                return context;

            if ((int)response.StatusCode >= 500)
            {
                throw new ProxyException("Proxy call result content is can not be null, " +
                                   "The exception may have occurred on the Server", null);
            }

            context.ResultContent = await response.Content.ReadAsStringAsync();

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
