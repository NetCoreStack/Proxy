using System.Reflection;

namespace NetCoreStack.Proxy
{
    public enum PropertyContentType
    {
        String = 0,
        Multipart = 1
    }

    public class PropertyContentTypeInfo
    {
        public PropertyInfo PropertyInfo { get; }
        public PropertyContentType PropertyContentType { get; }

        public PropertyContentTypeInfo(PropertyInfo propertyInfo, PropertyContentType contentType)
        {
            PropertyInfo = propertyInfo;
            PropertyContentType = contentType;
        }
    }
}
