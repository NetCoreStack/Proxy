using NetCoreStack.Proxy.Internal;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xunit;

namespace NetCoreStack.Proxy.Tests
{
    public class Model
    {
        public Guid Id { get; set; }
        public byte Byte { get; set; }
        public string String { get; set; }
        public IList<Guid> GuidList { get; set; }
        public DateTime Date { get; set; }
        public bool Boolean { get; set; }
        public int? Index { get; set; }
        public FooSingle FooSingle { get; set; }
        public Foo2[] Fooes { get; set; }
    }

    public class FooSingle
    {
        public string Str { get; set; }
        public IEnumerable<int> Ints { get; set; }
    }

    public class Foo2
    {
        public string Foo { get; set; }
    }

    public class modelTypeTests
    {
        [Fact]
        public void PropertyInfoTests()
        {
            SampleModel model = new SampleModel();
            model.Array = new int[] { 1, 2 };
            model.Boolean = true;
            model.Date = DateTime.Now;
            model.Number = 1;
            model.String = "string";
            model.Enumerable = new[] { 6, 7, 8 };

            Type modelType = model.GetType();

            ProxyModelMetadata modelMetadata = new ProxyModelMetadata(ProxyModelMetadataIdentity.ForType(modelType));
            IDictionary<string, PropertyInfo> properties = modelType.GetProperties().ToDictionary(p => p.Name, p => p);

            foreach (KeyValuePair<string, PropertyInfo> entry in properties)
            {
                var propertyMetadata = new ProxyModelMetadata(ProxyModelMetadataIdentity.ForProperty(entry.Value.PropertyType, entry.Key, modelType));
                Debug.WriteLine(entry.Key, entry.Value);
            }
        }
    }
}
