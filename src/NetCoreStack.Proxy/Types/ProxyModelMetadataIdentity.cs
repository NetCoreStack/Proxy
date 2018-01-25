using NetCoreStack.Proxy.Internal;
using System;
using System.Reflection;

namespace NetCoreStack.Proxy
{
    public struct ProxyModelMetadataIdentity : IEquatable<ProxyModelMetadataIdentity>
    {
        public static ProxyModelMetadataIdentity ForType(Type modelType)
        {
            if (modelType == null)
            {
                throw new ArgumentNullException(nameof(modelType));
            }

            return new ProxyModelMetadataIdentity()
            {
                ModelType = modelType,
            };
        }

        public static ProxyModelMetadataIdentity ForProperty(
            Type modelType,
            string name,
            Type containerType)
        {
            if (modelType == null)
            {
                throw new ArgumentNullException(nameof(modelType));
            }

            if (containerType == null)
            {
                throw new ArgumentNullException(nameof(containerType));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            return new ProxyModelMetadataIdentity()
            {
                ModelType = modelType,
                Name = name,
                ContainerType = containerType,
            };
        }

        public static ProxyModelMetadataIdentity ForParameter(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return new ProxyModelMetadataIdentity()
            {
                Name = parameter.Name,
                ModelType = parameter.ParameterType,
                ParameterInfo = parameter,
            };
        }

        public Type ContainerType { get; private set; }

        public Type ModelType { get; private set; }

        public ProxyModelMetadataKind MetadataKind
        {
            get
            {
                if (ParameterInfo != null)
                {
                    return ProxyModelMetadataKind.Parameter;
                }
                else if (ContainerType != null && Name != null)
                {
                    return ProxyModelMetadataKind.Property;
                }
                else
                {
                    return ProxyModelMetadataKind.Type;
                }
            }
        }

        public string Name { get; private set; }

        public ParameterInfo ParameterInfo { get; private set; }

        public bool Equals(ProxyModelMetadataIdentity other)
        {
            return
                ContainerType == other.ContainerType &&
                ModelType == other.ModelType &&
                Name == other.Name &&
                ParameterInfo == other.ParameterInfo;
        }
        
        public override bool Equals(object obj)
        {
            var other = obj as ProxyModelMetadataIdentity?;
            return other.HasValue && Equals(other.Value);
        }
        
        public override int GetHashCode()
        {
            var hash = new HashCodeCombiner();
            hash.Add(ContainerType);
            hash.Add(ModelType);
            hash.Add(Name, StringComparer.Ordinal);
            hash.Add(ParameterInfo);
            return hash;
        }
    }
}
