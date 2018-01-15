using NetCoreStack.Proxy.Internal;
using NetCoreStack.Proxy.Test.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace NetCoreStack.Proxy.Tests
{
    public class ModelTypeTests
    {
        [Fact]
        public void PropertyInfoTests()
        {
            //SampleModel model = new SampleModel();
            //model.Array = new int[] { 1, 2 };
            //model.Boolean = true;
            //model.Date = DateTime.Now;
            //model.Number = 1;
            //model.String = "string";
            //model.Enumerable = new[] { 6, 7, 8 };

            Type modelType = typeof(ComplexTypeModel);
            ProxyModelMetadata modelMetadata = new ProxyModelMetadata(ProxyModelMetadataIdentity.ForType(modelType));
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
