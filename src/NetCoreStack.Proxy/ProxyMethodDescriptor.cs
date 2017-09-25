using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Contracts;
using NetCoreStack.Proxy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy
{
    public class ProxyMethodDescriptor
    {
        public string Template { get; set; }
        public MethodInfo MethodInfo { get; }
        public Type ReturnType { get; }

        public Type UnderlyingReturnType { get; }

        public HttpMethod HttpMethod { get; set; }

        public TimeSpan? Timeout { get; set; }

        public List<ProxyParameterDescriptor> Parameters { get; set; }

        public bool IsVoidReturn { get; }
        public bool IsTaskReturn { get; }
        public bool IsGenericTaskReturn { get; }
        public bool IsActionResult { get; }

        public ProxyMethodDescriptor(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            ReturnType = methodInfo.ReturnType;
            IsVoidReturn = ReturnType == typeof(void);
            IsTaskReturn = ReturnType.IsAssignableFrom(typeof(Task)) ? true : false;
            IsGenericTaskReturn = ReturnType.IsGenericTask() ? true : false;
            IsActionResult = ReturnType.IsAssignableFrom(typeof(IActionResult));

            if (IsGenericTaskReturn)
            {
                UnderlyingReturnType = ReturnType.GetGenericArguments()[0];
            }
        }

        public IDictionary<string, object> Resolve(object[] args)
        {
            var values = new Dictionary<string, object>();
            if (HttpMethod == HttpMethod.Get)
            {
                // Ref type parameter resolver
                if (Parameters.Count == 1 && Parameters[0].ParameterType.IsReferenceType())
                {
                    var obj = args[0].ToDictionary();
                    values.Merge(obj, true);
                    return values;
                }

                if (Parameters.Count > 1 && Parameters.Any(x => x.ParameterType.IsReferenceType()))
                {
                    throw new ArgumentOutOfRangeException($"Methods marked with HTTP GET can take only one reference type parameter at the same time.");
                }
            }

            values.MergeArgs(args, Parameters.ToArray());
            return values;
        }
    }
}
