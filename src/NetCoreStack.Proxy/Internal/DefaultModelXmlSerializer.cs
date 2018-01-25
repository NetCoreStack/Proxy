using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NetCoreStack.Proxy.Internal
{
    public class DefaultModelXmlSerializer : IModelXmlSerializer
    {
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        public string Serialize(object value)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                NewLineHandling = NewLineHandling.None,
                Indent = false
            };

            Utf8StringWriter stringWriter = new Utf8StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter, settings);
            XmlSerializer xmlSerializer = new XmlSerializer(value.GetType());
            xmlSerializer.Serialize(writer, value);
            return stringWriter.ToString();
        }
    }
}