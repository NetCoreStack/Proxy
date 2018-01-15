using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NetCoreStack.Proxy.Internal
{
    public class ProxyModelMetadata: IEquatable<ProxyModelMetadata>
    {
        private int? _hashCode;
        
        public Type ModelType => Identity.ModelType;

        public Type ContainerType => Identity.ContainerType;

        public Type ElementType { get; private set; }

        public Type UnderlyingOrModelType { get; private set; }

        protected ProxyModelMetadataIdentity Identity { get; }        

        public ProxyModelMetadataKind MetadataKind => Identity.MetadataKind;

        public string PropertyName => Identity.Name;

        public bool IsComplexType { get; private set; }

        public bool IsNullableValueType { get; private set; }

        public bool IsCollectionType { get; private set; }

        public bool IsEnumerableType { get; private set; }
        
        public bool IsReferenceOrNullableType { get; private set; }
        
        public ProxyModelMetadata(ProxyModelMetadataIdentity identity)
        {
            Identity = identity;

            InitializeTypeInformation();
        }

        private void InitializeTypeInformation()
        {
            IsComplexType = !TypeDescriptor.GetConverter(ModelType).CanConvertFrom(typeof(string));
            IsNullableValueType = Nullable.GetUnderlyingType(ModelType) != null;
            IsReferenceOrNullableType = !ModelType.GetTypeInfo().IsValueType || IsNullableValueType;
            UnderlyingOrModelType = Nullable.GetUnderlyingType(ModelType) ?? ModelType;

            var collectionType = ClosedGenericMatcher.ExtractGenericInterface(ModelType, typeof(ICollection<>));
            IsCollectionType = collectionType != null;

            if (ModelType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(ModelType))
            {
                // Do nothing, not Enumerable.
            }
            else if (ModelType.IsArray)
            {
                IsEnumerableType = true;
                ElementType = ModelType.GetElementType();
            }
            else
            {
                IsEnumerableType = true;

                var enumerableType = ClosedGenericMatcher.ExtractGenericInterface(ModelType, typeof(IEnumerable<>));
                ElementType = enumerableType?.GenericTypeArguments[0];

                if (ElementType == null)
                {
                    // ModelType implements IEnumerable but not IEnumerable<T>.
                    ElementType = typeof(object);
                }
            }
        }

        public bool Equals(ProxyModelMetadata other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }
            else
            {
                return Identity.Equals(other.Identity);
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProxyModelMetadata);
        }
        
        public override int GetHashCode()
        {
            if (_hashCode == null)
            {
                _hashCode = Identity.GetHashCode();
            }

            return _hashCode.Value;
        }
    }
}
