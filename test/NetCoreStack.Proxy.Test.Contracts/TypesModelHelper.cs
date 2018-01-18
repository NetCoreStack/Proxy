using System;
using System.Globalization;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public static class TypesModelHelper
    {
        public static ComplexTypeModel GetComplexTypeModel()
        {
            var dateTime = DateTime.Parse("1/1/2018 9:00:00 AM", CultureInfo.InvariantCulture);
            var dateTimeOffset = new DateTimeOffset(dateTime);
            var guid = Guid.Parse("5cb98c19-7c0c-4661-bd9a-a2042b3bcb0d");
            ComplexTypeModel model = new ComplexTypeModel
            {
                Bar = new Bar
                {
                    String = "Bar string value!",
                    SomeEnum = SomeEnum.Value0,
                    someint = 6,
                    Foo = new Foo { IEnumerableInt = new[] { 2, 3 }, String = "Foo inner str value!" }
                },
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

            return model;
        }
    }
}
