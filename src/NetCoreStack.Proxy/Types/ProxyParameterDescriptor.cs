﻿namespace NetCoreStack.Proxy
{
    //public class ProxyParameterDescriptor
    //{
    //    public string Name { get; set; }
    //    public Type ParameterType { get; set; }
    //    public IDictionary<string, PropertyContentTypeInfo> Properties { get; }
    //    public bool HasFormFile { get; }

    //    public ProxyParameterDescriptor(List<PropertyInfo> properties)
    //    {
    //        Properties = new Dictionary<string, PropertyContentTypeInfo>(StringComparer.OrdinalIgnoreCase);

    //        foreach (var prop in properties)
    //        {
    //            PropertyContentType contentType = PropertyContentType.String;
    //            if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
    //            {
    //                HasFormFile = true;
    //                contentType = PropertyContentType.FormFile;
    //            }
    //            else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(prop.PropertyType))
    //            {
    //                HasFormFile = true;
    //                contentType = PropertyContentType.FormFileCollection;
    //            }
    //            else if (typeof(byte[]).IsAssignableFrom(prop.PropertyType))
    //            {
    //                contentType = PropertyContentType.ByteArray;
    //            }

    //            Properties.Add(prop.Name, new PropertyContentTypeInfo(prop, contentType));
    //        }
    //    }
    //}
}