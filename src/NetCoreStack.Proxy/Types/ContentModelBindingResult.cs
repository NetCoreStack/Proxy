using NetCoreStack.Proxy.Internal;
using System;
using System.Net.Http;

namespace NetCoreStack.Proxy
{
    public struct ContentModelBindingResult : IEquatable<ContentModelBindingResult>
    {
        public static ContentModelBindingResult Failed()
        {
            return new ContentModelBindingResult(content: null, isContentSet: false);
        }

        public static ContentModelBindingResult Success(HttpContent content)
        {
            return new ContentModelBindingResult(content, isContentSet: true);
        }

        private ContentModelBindingResult(HttpContent content, bool isContentSet)
        {
            Content = content;
            IsContentSet = isContentSet;
        }

        public HttpContent Content { get; }

        public bool IsContentSet { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as ContentModelBindingResult?;
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
            hashCodeCombiner.Add(IsContentSet);
            hashCodeCombiner.Add(Content);

            return hashCodeCombiner.CombinedHash;
        }

        public bool Equals(ContentModelBindingResult other)
        {
            return
                IsContentSet == other.IsContentSet &&
                object.Equals(Content, other.Content);
        }

        public override string ToString()
        {
            if (IsContentSet)
            {
                return $"Success '{Content}'";
            }
            else
            {
                return "Failed";
            }
        }

        public static bool operator ==(ContentModelBindingResult x, ContentModelBindingResult y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ContentModelBindingResult x, ContentModelBindingResult y)
        {
            return !x.Equals(y);
        }
    }
}