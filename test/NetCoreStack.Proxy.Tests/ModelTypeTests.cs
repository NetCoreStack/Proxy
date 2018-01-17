using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace NetCoreStack.Proxy.Tests
{
    public class ModelTypeTests
    {
        [Fact]
        public void ProxyModelMetadataInfoTests()
        {
            ComplexTypeModel complexTypeModel = new ComplexTypeModel
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

            var metadataProvider = new ProxyMetadataProvider();
            var modelMetadata = metadataProvider.GetMetadataForType(complexTypeModel.GetType());
            object[] args = new object[] { complexTypeModel };
            
            DefaultModelContentResolver contentResolver = new DefaultModelContentResolver(metadataProvider);
            ResolvedContentResult contentResult = contentResolver.Resolve(HttpMethod.Get, new List <ProxyModelMetadata> { modelMetadata }, false, args);
        }

        [Fact]
        public void ProxyModelMetadataInfoTests2()
        {
            ComplexTypeModel2 complexTypeModel2 = new ComplexTypeModel2
            {
                Bar = new Bar { String = "Bar string value", Foo = new Foo { IEnumerableInt = new[] { 2, 3 }, String = "Foo inner str value" } },
            };

            var metadataProvider = new ProxyMetadataProvider();
            var modelMetadata = metadataProvider.GetMetadataForType(complexTypeModel2.GetType());
            object[] args = new object[] { complexTypeModel2 };

            DefaultModelContentResolver contentResolver = new DefaultModelContentResolver(metadataProvider);
            ResolvedContentResult contentResult = contentResolver.Resolve(HttpMethod.Get, new List <ProxyModelMetadata> { modelMetadata }, false, args);
        }

        [Fact]
        public void ProxyModelMetadataInfoTests3()
        {
            ComplexTypeModel3 complexTypeModel3 = new ComplexTypeModel3
            {
                String = "string value!",
                Bar = new Bar { String = "Bar string value", Foo = new Foo { IEnumerableInt = new[] { 2, 3 }, String = "Foo inner str value" } },
                Foo = new Foo { IEnumerableInt = new[] { 4, 5 }, String = "Foo string value" }
            };

            var metadataProvider = new ProxyMetadataProvider();
            var modelMetadata = metadataProvider.GetMetadataForType(complexTypeModel3.GetType());
            object[] args = new object[] { complexTypeModel3 };

            DefaultModelContentResolver contentResolver = new DefaultModelContentResolver(metadataProvider);
            ResolvedContentResult contentResult = contentResolver.Resolve(HttpMethod.Get, new List <ProxyModelMetadata> { modelMetadata }, false, args);


            var expect = new Dictionary<string, string>()
            {
                ["String"] = "string value!",
                ["Foo.String"] = "Foo string value",
                ["Foo.IEnumerableInt[0]"] = "4",
                ["Foo.IEnumerableInt[1]"] = "5",
                ["Bar.String"] = "Bar string value",
                ["Bar.Foo.String"] = "Foo inner str value",
                ["Bar.Foo.IEnumerableInt[0]"] = "2",
                ["Bar.Foo.IEnumerableInt[1]"] = "3",
            };


            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(8, expect.Count);
        }
    }
}
