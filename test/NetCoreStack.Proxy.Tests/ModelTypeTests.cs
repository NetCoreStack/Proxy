using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Xunit;

namespace NetCoreStack.Proxy.Tests
{
    public class ModelTypeTests
    {
        private readonly ComplexTypeModel _complexTypeModel = new ComplexTypeModel
        {
            Bar = new Bar { String = "Bar string value", Foo = new Foo { IEnumerableInt = new[] { 2, 3 }, String = "Foo inner str value" } },
            Bool = true,
            Foo = new Foo { IEnumerableInt = new[] { 4, 5 }, String = "Foo string value" },
            Byte = (byte)'A', // 65
            Char = 'c',
            DateTime = DateTime.Now,
            DateTimeOffset = new DateTimeOffset(DateTime.UtcNow),
            Decimal = 300.5m,
            DecimalNullable = null,
            Double = 1.7E+3, // 1700
            Float = 4.5f,
            Guid = Guid.NewGuid(),
            IEnumerable = new[] { "value1", "value2", "value3" },
            ICollection = new[] { 2, 4, 6, 8 },
            Int = int.MaxValue,
            IntArray = new int[] { int.MaxValue, int.MinValue },
            Long = 0x100000000, // 4294967296
            Object = new { a = 1, b = true, c = "str" },
            SByte = -102,
            Short = 0x040A, // 1034
            String = "string value!",
            TimeSpan = new TimeSpan(4, 15, 30),
            UInt = 0xB2D05E00, // 3000000000
            ULong = 0x0001D8e864DD, // 7934076125
            Uri = new Uri("http://localhost:5003"),
            UShort = 0xFE0A // 65034
        };

        private readonly ComplexTypeModel2 _complexTypeModel2 = new ComplexTypeModel2
        {
            // String = "string value!",
            Bar = new Bar { String = "Bar string value", Foo = new Foo { IEnumerableInt = new[] { 2, 3 }, String = "Foo inner str value" } },
            // Foo = new Foo { IEnumerableInt = new[] { 4, 5 }, String = "Foo string value" }
        };

        [Fact]
        public void PropertyInfoTests()
        {            
            var metadataProvider = new ProxyMetadataProvider();
            var modelMetadata = metadataProvider.GetMetadataForType(typeof(ComplexTypeModel2));
            object[] args = new object[] { _complexTypeModel2 };
            
            DefaultModelContentResolver contentResolver = new DefaultModelContentResolver(metadataProvider);
            ResolvedContentResult contentResult = contentResolver.Resolve(new List<ProxyModelMetadata> { modelMetadata }, HttpMethod.Get, false, args);

            var counter = 0;
            foreach (var metadata in modelMetadata.Properties)
            {
                List<string> list = new List<string>();
                if (metadata.IsSimpleType)
                {
                    list.Add("Simple Type");
                }

                if (metadata.IsComplexType)
                {
                    list.Add("Complex Type");
                }

                if (metadata.IsCollectionType)
                {
                    list.Add("Collection Type");
                }

                if (metadata.IsNullableValueType)
                {
                    list.Add("NullableType Value Type");
                }

                if (metadata.IsReferenceOrNullableType)
                {
                    list.Add("Reference Or Nullable Type");
                }

                if (metadata.ModelType.IsArray)
                {
                    list.Add("Array Type");
                }

                if (metadata.IsEnumerableType)
                {
                    list.Add("Element Type:" + metadata.ElementType.Name);
                }

                Debug.WriteLine(counter + "-" + metadata.PropertyName + ": " + string.Join(",", list));
                counter++;
            }
        }
    }
}
