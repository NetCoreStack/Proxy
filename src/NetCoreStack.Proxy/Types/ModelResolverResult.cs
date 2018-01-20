using NetCoreStack.Proxy.Internal;
using System;

namespace NetCoreStack.Proxy
{
    public struct ModelResolverResult : IEquatable<ModelResolverResult>
    {
        public bool IsValueSet { get; }

        public static ModelResolverResult Failed()
        {
            return new ModelResolverResult(isValueSet: false);
        }

        public static ModelResolverResult Success()
        {
            return new ModelResolverResult(isValueSet: true);
        }

        private ModelResolverResult(bool isValueSet)
        {
            IsValueSet = isValueSet;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ModelResolverResult?;
            if (other == null)
            {
                return false;
            }
            else
            {
                return Equals(other.Value);
            }
        }

        public override int GetHashCode()
        {
            var hashCodeCombiner = HashCodeCombiner.Start();
            hashCodeCombiner.Add(IsValueSet);
            return hashCodeCombiner.CombinedHash;
        }

        public bool Equals(ModelResolverResult other)
        {
            return IsValueSet == other.IsValueSet;
        }

        public override string ToString()
        {
            if (IsValueSet)
            {
                return $"Success";
            }
            else
            {
                return "Failed";
            }
        }

        public static bool operator ==(ModelResolverResult x, ModelResolverResult y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ModelResolverResult x, ModelResolverResult y)
        {
            return !x.Equals(y);
        }
    }
}
