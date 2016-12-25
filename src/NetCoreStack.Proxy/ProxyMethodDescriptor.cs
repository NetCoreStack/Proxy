using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using NetCoreStack.Common;
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
        public MethodInfo MethodInfo { get; }
        public Type ReturnType { get; }

        public Type UnderlyingReturnType { get; }

        public HttpMethod HttpMethod { get; set; }

        public TimeSpan? Timeout { get; set; }

        public List<ParameterDescriptor> Parameters { get; set; }

        public bool IsDirectStreamTransport
        {
            get
            {
                return ReturnType.IsDirectStreamTransport();
            }
        }

        public bool IsVoidReturn { get; }
        public bool IsTaskReturn { get; }

        public bool IsGenericTaskReturn { get; }
        public bool IsActionResult { get; }

        public bool IsCollectionResult
        {
            get
            {
                if (ReturnType.IsAssignableFrom(typeof(CollectionResult)))
                {
                    return true;
                }

                return false;
            }
        }

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
                if (args.Length > 1)
                    throw new ArgumentOutOfRangeException($"Methods marked with HTTP GET can take one reference type parameter.");

                if (args.Length != 0)
                {
                    var obj = args[0].ToDictionary();
                    values.Merge(obj, true);
                }
            }
            else
                values.MergeArgs(args, Parameters.ToArray());

            return values;
        }
    }
}
