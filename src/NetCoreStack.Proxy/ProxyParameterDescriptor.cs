using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public class ProxyParameterDescriptor : ParameterDescriptor
    {
        public IDictionary<string, PropertyContentTypeInfo> Properties { get; }
        
        public bool HasFormFile { get; }

        public ProxyParameterDescriptor(List<PropertyInfo> properties)
        {
            Properties = new Dictionary<string, PropertyContentTypeInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var prop in properties)
            {
                PropertyContentType contentType = PropertyContentType.String;
                if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                {
                    HasFormFile = true;
                    contentType = PropertyContentType.Multipart;
                }

                Properties.Add(prop.Name, new PropertyContentTypeInfo(prop, contentType));
            }
        }
    }
}
