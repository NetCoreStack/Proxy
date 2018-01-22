using Microsoft.AspNetCore.Http;
using NetCoreStack.Proxy.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public class ProxyModelMetadata: IEquatable<ProxyModelMetadata>
    {
        private int? _hashCode;
        protected ProxyModelMetadataIdentity Identity { get; }

        public Type ModelType => Identity.ModelType;

        public Type ContainerType => Identity.ContainerType;

        public ProxyModelMetadata ElementType { get; private set; }

        public Type UnderlyingOrModelType { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        public ProxyModelMetadataKind MetadataKind => Identity.MetadataKind;

        public string PropertyName => Identity.Name;

        public bool IsFormFile { get; private set; }

        public bool IsSimpleType { get; private set; }

        public bool IsSystemObject { get; private set; }

        public bool IsComplexType { get; private set; }

        public bool IsNullableValueType { get; private set; }

        public bool IsCollectionType { get; private set; }

        public bool IsEnumerableType { get; private set; }

        public bool IsReferenceType { get; private set; }

        public bool IsReferenceOrNullableType { get; private set; }

        public IList<ProxyModelMetadata> Properties { get; private set; }

        public int PropertiesCount => Properties.Count;

        public ProxyModelMetadata(ProxyModelMetadataIdentity identity)
        {
            Identity = identity;
            Properties = new List<ProxyModelMetadata>();
            InitializeTypeInformation();
        }

        public ProxyModelMetadata(PropertyInfo propertyInfo, ProxyModelMetadataIdentity identity)
            :this(identity)
        {
            PropertyInfo = propertyInfo;
        }

        private bool IsSimple(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return typeInfo.IsPrimitive ||
                typeInfo.IsEnum ||
                type.Equals(typeof(decimal)) ||
                type.Equals(typeof(string)) ||
                type.Equals(typeof(DateTime)) ||
                type.Equals(typeof(Guid)) ||
                type.Equals(typeof(DateTimeOffset)) ||
                type.Equals(typeof(TimeSpan)) ||
                type.Equals(typeof(Uri));
        }

        private void InitializeTypeInformation()
        {
            var typeInfo = ModelType.GetTypeInfo();

            IsComplexType = !TypeDescriptor.GetConverter(ModelType).CanConvertFrom(typeof(string));
            IsNullableValueType = Nullable.GetUnderlyingType(ModelType) != null;
            IsReferenceType = !typeInfo.IsValueType;
            IsReferenceOrNullableType = !typeInfo.IsValueType || IsNullableValueType;
            UnderlyingOrModelType = Nullable.GetUnderlyingType(ModelType) ?? ModelType;
            IsFormFile = typeof(IFormFile).IsAssignableFrom(ModelType);
            IsSimpleType = IsSimple(ModelType);
            IsSystemObject = ModelType == typeof(object);

            var collectionType = ClosedGenericMatcher.ExtractGenericInterface(ModelType, typeof(ICollection<>));
            IsCollectionType = collectionType != null;

            if (ModelType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(ModelType))
            {
                // Do nothing, not Enumerable.
            }
            else if (ModelType.IsArray)
            {
                IsEnumerableType = true;
                var elementType = ModelType.GetElementType();

                PropertyInfo propertyInfo = ContainerType?.GetProperty(PropertyName);
                if (propertyInfo != null)
                {
                    ElementType = new ProxyModelMetadata(propertyInfo, ProxyModelMetadataIdentity.ForProperty(elementType, propertyInfo.Name, ModelType));
                }

                IsFormFile = typeof(IFormFile).IsAssignableFrom(elementType);
            }
            else
            {
                IsEnumerableType = true;

                var enumerableType = ClosedGenericMatcher.ExtractGenericInterface(ModelType, typeof(IEnumerable<>));
                var elementType = enumerableType?.GenericTypeArguments[0];

                if (elementType != null)
                {
                    IsFormFile = typeof(IFormFile).IsAssignableFrom(elementType);
                }

                if (elementType == null)
                {
                    // ModelType implements IEnumerable but not IEnumerable<T>.
                    elementType = typeof(object);
                }

                PropertyInfo propertyInfo = ContainerType?.GetProperty(PropertyName);
                if (propertyInfo != null)
                {
                    ElementType = new ProxyModelMetadata(propertyInfo, ProxyModelMetadataIdentity.ForProperty(elementType, propertyInfo.Name, ModelType));
                }
            }

            if (IsNullableValueType)
            {
                var elementType = Nullable.GetUnderlyingType(ModelType);
                PropertyInfo pInfo = ContainerType?.GetProperty(PropertyName);
                if (pInfo != null)
                {
                    ElementType = new ProxyModelMetadata(pInfo, ProxyModelMetadataIdentity.ForProperty(elementType, pInfo.Name, ModelType));
                }
            }

            if (IsComplexType && IsReferenceOrNullableType && 
                (!IsEnumerableType && !IsCollectionType) &&
                ModelType.Name != typeof(object).Name)
            {
                PropertyInfo[] properties = ModelType.GetProperties();
                List<ProxyModelMetadata> metadataList = new List<ProxyModelMetadata>();
                foreach (var prop in properties)
                {
                    metadataList.Add(new ProxyModelMetadata(prop, ProxyModelMetadataIdentity.ForProperty(prop.PropertyType, prop.Name, ModelType)));
                }

                Properties = metadataList;
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

        public override string ToString()
        {
            var name = PropertyName;
            if (ContainerType != null)
            {
                name = ContainerType.Name + " - " + name;
            }

            return name;
        }
    }
}
