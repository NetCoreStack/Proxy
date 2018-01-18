using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using Xunit;

namespace NetCoreStack.Proxy.Tests
{
    public class ModelTypeTests
    {
        private ResolvedContentResult GetResolvedContentResult(object model, bool isMultipartFormData)
        {
            var metadataProvider = new ProxyMetadataProvider();
            var modelMetadata = metadataProvider.GetMetadataForType(model.GetType());
            object[] args = new object[] { model };

            DefaultModelContentResolver contentResolver = new DefaultModelContentResolver(metadataProvider);
            return contentResolver.Resolve(HttpMethod.Get, new List<ProxyModelMetadata> { modelMetadata }, isMultipartFormData, args);
        }

        [Fact]
        public void ProxyModelMetadataInfoComplexTypeModelTests()
        {
            var dateTime = DateTime.Parse("1/1/2018 9:00:00 AM", CultureInfo.InvariantCulture);
            var dateTimeOffset = new DateTimeOffset(dateTime);
            var guid = Guid.Parse("5cb98c19-7c0c-4661-bd9a-a2042b3bcb0d");
            ComplexTypeModel model = new ComplexTypeModel
            {
                Bar = new Bar { String = "Bar string value!", SomeEnum = SomeEnum.Value0, someint = 6,
                    Foo = new Foo { IEnumerableInt = new[] { 2, 3 }, String = "Foo inner str value!" } },
                Bool = true,
                Foo = new Foo { IEnumerableInt = new[] { 4, 5 }, String = "Foo string value!" },
                Byte = (byte)'A', // 65
                Char = 'c',
                DateTime = dateTime,
                DateTimeOffset = dateTimeOffset,
                Decimal = 300.5m, // 300.5
                DecimalNullable = 100.5m,
                Double = 1.7E+3, // 1700
                Float = 4.5f, // 4.5
                Guid = guid,
                IEnumerable = new[] { "value1", "value2", "value3" },
                ICollection = new[] { 2, 4, 6, 8 },
                Int = int.MaxValue,
                IntArray = new int[] { int.MaxValue, int.MinValue },
                Long = 0x100000000, // 4294967296
                // Object = new { a = 1, b = true, c = "str" },
                SByte = -102,
                Short = 0x040A, // 1034
                String = "string value!",
                TimeSpan = new TimeSpan(4, 15, 30),
                UInt = 0xB2D05E00, // 3000000000
                ULong = 0x0001D8e864DD, // 7934076125
                Uri = new Uri("http://localhost:5003"),
                UShort = 0xFE0A // 65034
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["Bar.String"] = "Bar string value!",
                ["Bar.SomeEnum"] = "Value0",
                ["Bar.someint"] = "6",
                ["Bar.Foo.IEnumerableInt[0]"] = "2",
                ["Bar.Foo.IEnumerableInt[1]"] = "3",
                ["Bar.Foo.String"] = "Foo inner str value!",
                ["Bool"] = "True",
                ["Foo.IEnumerableInt[0]"] = "4",
                ["Foo.IEnumerableInt[1]"] = "5",
                ["Foo.String"] = "Foo string value!",
                ["Byte"] = "65",
                ["Char"] = "c",
                ["DateTime"] = "1/1/2018 9:00:00 AM",
                ["DateTimeOffset"] = "1/1/2018 9:00:00 AM +03:00",
                ["Decimal"] = "300.5",
                ["DecimalNullable"] = "100.5",
                ["Double"] = "1700",
                ["Float"] = "4.5",
                ["Guid"] = "5cb98c19-7c0c-4661-bd9a-a2042b3bcb0d",
                ["IEnumerable[0]"] = "value1",
                ["IEnumerable[1]"] = "value2",
                ["IEnumerable[2]"] = "value3",
                ["ICollection[0]"] = "2",
                ["ICollection[1]"] = "4",
                ["ICollection[2]"] = "6",
                ["ICollection[3]"] = "8",
                ["Int"] = "2147483647",
                ["IntArray[0]"] = "2147483647",
                ["IntArray[1]"] = "-2147483648",
                ["Long"] = "4294967296",
                // ["Object"] = "",
                ["SByte"] = "-102",
                ["Short"] = "1034",
                ["String"] = "string value!",
                ["TimeSpan"] = "04:15:30",
                ["UInt"] = "3000000000",
                ["ULong"] = "7934076125",
                ["Uri"] = "http://localhost:5003",
                ["UShort"] = "65034"
            };

            var actual = contentResult.Dictionary;
            foreach (KeyValuePair<string, string> entry in expect)
            {
                if (actual.TryGetValue(entry.Key, out string value))
                {
                    // noop
                }
                else
                {
                    Debug.WriteLine($"Missing Key: {entry.Key}");
                }
            }

            Assert.Equal(expect, actual);
            Assert.Equal(37, actual.Count);
        }

        [Fact]
        public void ProxyModelMetadataInfoTests2()
        {
            ComplexTypeModel2 model = new ComplexTypeModel2
            {
                Bar = new Bar { String = "Bar string value", SomeEnum = SomeEnum.Value1, someint = 6, Foo = new Foo { IEnumerableInt = new[] { 2, 3 }, String = "Foo inner str value" } },
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["Bar.String"] = "Bar string value",
                ["Bar.someint"] = "6",
                ["Bar.SomeEnum"] = "Value1",
                ["Bar.Foo.String"] = "Foo inner str value",
                ["Bar.Foo.IEnumerableInt[0]"] = "2",
                ["Bar.Foo.IEnumerableInt[1]"] = "3",
            };

            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(6, contentResult.Dictionary.Count);
        }

        [Fact]
        public void ProxyModelMetadataInfoTests3()
        {
            ComplexTypeModel3 model = new ComplexTypeModel3
            {
                String = "string value!",
                Bar = new Bar { String = "Bar string value", SomeEnum = SomeEnum.Value4, someint = 6, Foo = new Foo { IEnumerableInt = new[] { 2, 3 }, String = "Foo inner str value" } },
                Foo = new Foo { IEnumerableInt = new[] { 4, 5 }, String = "Foo string value" }
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["String"] = "string value!",
                ["Foo.String"] = "Foo string value",
                ["Foo.IEnumerableInt[0]"] = "4",
                ["Foo.IEnumerableInt[1]"] = "5",
                ["Bar.String"] = "Bar string value",
                ["Bar.someint"] = "6",
                ["Bar.SomeEnum"] = "Value4",
                ["Bar.Foo.String"] = "Foo inner str value",
                ["Bar.Foo.IEnumerableInt[0]"] = "2",
                ["Bar.Foo.IEnumerableInt[1]"] = "3",
            };

            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(10, contentResult.Dictionary.Count);
        }

        [Fact]
        public void ProxyModelMetadataInfoForFooModel()
        {
            Foo model = new Foo
            {
                String = "Foo string value!",
                IEnumerableInt = new[] { 1, 3, 5, 7, 9 }
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["String"] = "Foo string value!",
                ["IEnumerableInt[0]"] = "1",
                ["IEnumerableInt[1]"] = "3",
                ["IEnumerableInt[2]"] = "5",
                ["IEnumerableInt[3]"] = "7",
                ["IEnumerableInt[4]"] = "9"
            };
            
            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(6, contentResult.Dictionary.Count);
        }

        [Fact]
        public void ProxyModelMetadataInfoForBarModel()
        {
            Bar model = new Bar
            {
                String = "Bar string value!",
                someint = 6,
                SomeEnum = SomeEnum.Value2,
                Foo = new Foo { IEnumerableInt = new[] { 1, 3, 5, 7, 9 }, String = "Foo string value!" }
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["String"] = "Bar string value!",
                ["someint"] = "6",
                ["SomeEnum"] = "Value2",
                ["Foo.String"] = "Foo string value!",
                ["Foo.IEnumerableInt[0]"] = "1",
                ["Foo.IEnumerableInt[1]"] = "3",
                ["Foo.IEnumerableInt[2]"] = "5",
                ["Foo.IEnumerableInt[3]"] = "7",
                ["Foo.IEnumerableInt[4]"] = "9"
            };

            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(9, contentResult.Dictionary.Count);
        }

        [Fact]
        public void ProxyModelMetadataInfoForStringEnumerableModel()
        {
            StringEnumerable model = new StringEnumerable
            {
                IEnumerableString = new[] { "One", "Two", "Three", "Four", "Five" }
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["IEnumerableString[0]"] = "One",
                ["IEnumerableString[1]"] = "Two",
                ["IEnumerableString[2]"] = "Three",
                ["IEnumerableString[3]"] = "Four",
                ["IEnumerableString[4]"] = "Five"
            };

            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(5, contentResult.Dictionary.Count);
        }

        [Fact]
        public void ProxyModelMetadataInfoForUriEnumerableModel()
        {
            UriEnumerable model = new UriEnumerable
            {
                IEnumerableUri = new[] { new Uri("http://localhost:5003"), new Uri("http://localhost:5004"), new Uri("http://localhost:5005") }
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["IEnumerableUri[0]"] = "http://localhost:5003/",
                ["IEnumerableUri[1]"] = "http://localhost:5004/",
                ["IEnumerableUri[2]"] = "http://localhost:5005/",
            };

            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(3, contentResult.Dictionary.Count);
        }

        //[Fact]
        //public void ProxyModelMetadataInfoForObjectModel()
        //{
        //    ObjectModel model = new ObjectModel
        //    {
        //        Object = new[] { new Uri("http://localhost:5003"), new Uri("http://localhost:5004"), new Uri("http://localhost:5005") }
        //    };

        //    var contentResult = GetResolvedContentResult(model, false);

        //    var expect = new Dictionary<string, string>()
        //    {
        //        ["Object[0]"] = "http://localhost:5003/",
        //        ["Object[1]"] = "http://localhost:5004/",
        //        ["Object[2]"] = "http://localhost:5005/",
        //    };

        //    Assert.Equal(expect, contentResult.Dictionary);
        //    Assert.Equal(3, expect.Count);
        //}

        [Fact]
        public void ProxyModelMetadataInfoForNullableObjectModel()
        {
            var dateTime = DateTime.Parse("1/1/2018 9:00:00 AM", CultureInfo.InvariantCulture);
            NullableObjectModel model = new NullableObjectModel
            {
                DecimalNullable = 100.5m,
                DateTimeNullable = dateTime,
                IntNullable = 1
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["IntNullable"] = "1",
                ["DecimalNullable"] = "100.5",
                ["DateTimeNullable"] = "1/1/2018 9:00:00 AM",
            };

            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(3, contentResult.Dictionary.Count);
        }

        [Fact]
        public void ProxyModelMetadataInfoForPureEnumerable()
        {
            var dateTime = DateTime.Parse("1/1/2018 9:00:00 AM", CultureInfo.InvariantCulture);
            PureEnumerable model = new PureEnumerable
            {
                IEnumerable = new[] { "value1", "value2", "value3" }
            };

            var contentResult = GetResolvedContentResult(model, false);

            var expect = new Dictionary<string, string>()
            {
                ["IEnumerable[0]"] = "value1",
                ["IEnumerable[1]"] = "value2",
                ["IEnumerable[2]"] = "value3",
            };

            Assert.Equal(expect, contentResult.Dictionary);
            Assert.Equal(3, contentResult.Dictionary.Count);
        }
    }
}