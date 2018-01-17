using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class ObjectExtensions
    {
        //public static string ToQueryString(this object obj)
        //{
        //    var properties = from p in obj.GetType().GetProperties()
        //                     where p.GetValue(obj, null) != null
        //                     select p.Name + "=" + WebUtility.UrlEncode(p.GetValue(obj, null).ToString());

        //    return string.Join("&", properties.ToArray());
        //}

        public static IDictionary<string, object> ToDictionary(this object value)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            if (value != null)
            {
                PropertyInfo[] properties = value.GetType().GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo propertyInfo = properties[i];
                    dictionary.Add(propertyInfo.Name.Replace("_", "-"), propertyInfo.GetValue(value));
                }
            }
            return dictionary;
        }
    }
}
