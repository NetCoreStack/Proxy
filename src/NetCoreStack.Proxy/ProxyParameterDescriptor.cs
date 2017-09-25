using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public class ProxyParameterDescriptor : ParameterDescriptor
    {
        public List<PropertyInfo> Properties { get; }

        public bool EnsureMultipartFormData { get; }



        public ProxyParameterDescriptor(List<PropertyInfo> properties)
        {
            Properties = properties;
            EnsureMultipartFormData = Properties
                .Where(p => typeof(IFormFile).IsAssignableFrom(p.PropertyType) ||
                typeof(byte[]).IsAssignableFrom(p.PropertyType)).Any();
        }
    }
}
