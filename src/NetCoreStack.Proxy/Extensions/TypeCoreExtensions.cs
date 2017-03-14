using NetCoreStack.Common;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class TypeCoreExtensions
    {
        internal static Type BaseType(this Type type)
        {
            return IntrospectionExtensions.GetTypeInfo(type).BaseType;
        }

        internal static bool IsValueType(this Type type)
        {
            return IntrospectionExtensions.GetTypeInfo(type).IsValueType;
        }

        internal static bool IsGenericType(this Type type)
        {
            return IntrospectionExtensions.GetTypeInfo(type).IsGenericType;
        }

        internal static bool ContainsGenericParameters(this Type type)
        {
            return IntrospectionExtensions.GetTypeInfo(type).ContainsGenericParameters;
        }

        internal static bool IsGenericTask(this Type type)
        {
            if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Task<>))
                return true;

            return false;
        }

        internal static bool IsDirectStreamTransport(this Type returnType)
        {
            var result = false;

            if (returnType.IsAssignableFrom(typeof(CollectionResult)))
            {
                result = true;
            }

            return result;
        }
    }
}
