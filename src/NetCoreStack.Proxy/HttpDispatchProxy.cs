using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Proxy.Extensions;
using NetCoreStack.Proxy.Internal;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public class HttpDispatchProxy : DispatchProxyAsync
    {
        private ProxyContext _proxyContext;

        private IProxyManager _proxyManager;

        public HttpDispatchProxy()
        {
        }

        private object CreateContextProperties(RequestDescriptor descriptor, MethodInfo targetMethod)
        {
            var properties = new
            {
                EnvironmentName = Environment.MachineName,
                Message = $"Proxy call from Sandbox: {Environment.MachineName} to API method: {targetMethod?.Name}",
                Method = descriptor?.Request?.Method,
                Authority = descriptor?.Request?.RequestUri?.Authority,
                LocalPath = descriptor?.Request?.RequestUri?.LocalPath,
                RequestRegion = descriptor?.RegionKey
            };

            return properties;
        }

        /// <summary>
        /// Call after resolve the proxy
        /// </summary>
        /// <param name="proxyFactory"></param>
        public void Initialize(ProxyContext context, IProxyManager proxyManager)
        {
            _proxyContext = context;
            _proxyManager = proxyManager;
        }

        protected object GetProxy()
        {
            return this;
        }

        private async Task<ResponseContext> InternalInvokeAsync(MethodInfo targetMethod, object[] args, Type genericReturnType = null)
        {
            var requestContext = new RequestContext(targetMethod,
                _proxyContext.ProxyType,
                _proxyContext.ClientIp,
                _proxyContext.UserAgent,
                _proxyContext.TokenCookie,
                _proxyContext.QueryString,
                args);

            RequestDescriptor descriptor = await _proxyManager.CreateDescriptorAsync(requestContext);
            ResponseContext responseContext = null;
            try
            {
                HttpResponseMessage response = null;
                var httpClient = _proxyManager.HttpClient;
                string metadata = string.Empty;
                response = await httpClient.SendAsync(descriptor.Request);
                responseContext = await ProxyResultExecutor.ExecuteAsync(response,
                    descriptor,
                    genericReturnType);
            }
            catch (Exception ex)
            {
                var properties = CreateContextProperties(descriptor, targetMethod);
                var result = properties.GetConfigurationContextDetail(properties);
                throw new ProxyException(result, ex);
            }

            if ((int)responseContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var properties = CreateContextProperties(descriptor, targetMethod);
                var result = properties.GetConfigurationContextDetail(properties);
                throw new ProxyException(result, new HttpRequestException());
            }

            return responseContext;
        }

        public override async Task InvokeAsync(MethodInfo method, object[] args)
        {
            if (method.Name == "get_ControllerContext")
            {
                throw new NotSupportedException($"\"{nameof(ActionContext)}\" is not supported for proxy instance");
            }

            await InternalInvokeAsync(method, args);
        }

        public override async Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
        {
            if (method.Name == "get_ControllerContext")
            {
                throw new NotSupportedException($"\"{nameof(ActionContext)}\" is not supported for proxy instance");
            }

            var responseContext = await InternalInvokeAsync(method, args, typeof(T));
            var methodDescriptor = responseContext.RequestDescriptor.MethodDescriptor;
            return (T)responseContext.Value;
        }

        public override object Invoke(MethodInfo method, object[] args)
        {
            if (method.Name == "get_ControllerContext")
            {
                throw new NotSupportedException($"\"{nameof(ActionContext)}\" is not supported for proxy instance");
            }

            ResponseContext context = Task.Run(async () => await InternalInvokeAsync(method, args).ConfigureAwait(false)).Result;
            return context.Value;
        }
    }
}
