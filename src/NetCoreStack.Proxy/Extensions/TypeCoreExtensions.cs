using NetCoreStack.Common;
using System;
using System.Linq;
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

        internal static Type GetTaskType(this Type type)
        {
            if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Task<>))
                return type.GetGenericArguments()[0];

            throw new Exception("Type " + type.FullName + " is not an instance of Task<T>");
        }

        internal static PropertyInfo GetProperty(this MethodInfo method)
        {
            var hasReturn = method.ReturnType != typeof(void);
            Func<PropertyInfo, bool> predicate = prop => prop.GetSetMethod() == method;

            if (hasReturn)
                predicate = prop => prop.GetGetMethod() == method;

            return method.DeclaringType.GetProperties().FirstOrDefault(predicate);
        }

        internal static bool IsDirectStreamTransport(this Type returnType)
        {
            var result = false;

            if (returnType.IsAssignableFrom(typeof(CollectionResult)))
            {
                result = true;
            }

            if (returnType.IsGenericType())
            {

            }

            return result;
        }
    }
}
