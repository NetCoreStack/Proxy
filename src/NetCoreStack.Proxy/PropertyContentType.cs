using System.Reflection;

namespace NetCoreStack.Proxy
{
    public enum PropertyContentType
    {
        String = 0,
        FormFile = 1,
        FormFileCollection = 2,
        ByteArray = 3
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
