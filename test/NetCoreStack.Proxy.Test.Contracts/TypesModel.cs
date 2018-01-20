using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public class TypesModel
    {
        public bool Bool { get; set; }
        public byte Byte { get; set; }
        public sbyte SByte { get; set; }
        public char Char { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public float Float { get; set; }
        public int Int { get; set; }
        public uint UInt { get; set; }
        public long Long { get; set; }
        public ulong ULong { get; set; }
        public object Object { get; set; }
        public short Short { get; set; }
        public ushort UShort { get; set; }
        public string String { get; set; }
    }

    public class ComplexTypeModel : TypesModel
    {
        public int[] IntArray { get; set; }
        public decimal? DecimalNullable { get; set; }
        public IEnumerable IEnumerable { get; set; }
        public ICollection ICollection { get; set; }
        public Guid Guid { get; set; }
        public Uri Uri { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public Foo Foo { get; set; }
        public Bar Bar { get; set; }
    }

    public class ComplexTypeModel2
    {
        public Bar Bar { get; set; }
    }

    public class ComplexTypeModel3
    {
        public string String { get; set; }
        public Foo Foo { get; set; }
        public Bar Bar { get; set; }
    }

    public class ComplextTypeModelWithFiles : ComplexTypeModel
    {
        public IEnumerable<IFormFile> Files { get; set; }
    }

    public enum SomeEnum
    {
        Value0 = 0,
        Value1 = 1,
        Value2 = 2,
        Value4 = 4
    }

    public class Foo
    {
        public string String { get; set; }
        public IEnumerable<int> IEnumerableInt { get; set; }
    }

    public class Bar
    {
        public string String { get; set; }
        public int someint { get; set; }
        public SomeEnum SomeEnum { get; set; }
        public Foo Foo { get; set; }
    }

    public class ObjectModel
    {
        public object Object{ get; set; }
    }

    public class NullableObjectModel
    {
        public int? IntNullable { get; set; }
        public decimal? DecimalNullable { get; set; }
        public DateTime? DateTimeNullable { get; set; }
    }

    public class PureEnumerable
    {
        public IEnumerable IEnumerable { get; set; }
    }

    public class PureCollection
    {
        public ICollection ICollection { get; set; }
    }

    public class StringEnumerable
    {
        public IEnumerable<string> IEnumerableString { get; set; }
    }

    public class UriEnumerable
    {
        public IEnumerable<Uri> IEnumerableUri { get; set; }
    }

    public class SingleFileModel
    {
        public IFormFile File { get; set; }
    }

    public class EnumerableFileModel
    {
        public IEnumerable<IFormFile> Files { get; set; }
    }

    public class InnerFileModel
    {
        public IEnumerable<IFormFile> Files { get; set; }
    }

    public class FileModel
    {
        public InnerFileModel InnerFileModel { get; set; }
    }
}