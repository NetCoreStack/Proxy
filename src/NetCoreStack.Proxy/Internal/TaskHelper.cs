using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Internal
{
    internal static class TaskHelper
    {
        internal static object CreateFromResult(Type returnType)
        {
            MethodInfo mi = typeof(Task).GetMethod("FromResult");
            MethodInfo genericMethod = mi.MakeGenericMethod(returnType);
            return genericMethod.Invoke(null, null);
        }
    }
}
