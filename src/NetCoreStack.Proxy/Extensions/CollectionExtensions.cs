using System.Collections.Generic;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class CollectionExtensions
    {
        internal static IEnumerable<ProxyModelMetadata> ToFlat(this ProxyModelMetadata modelMetadata)
        {
            List<ProxyModelMetadata> list = new List<ProxyModelMetadata>();
            var stack = new Stack<IEnumerator<ProxyModelMetadata>>();
            var enumerator = modelMetadata.Properties.GetEnumerator();
            while (true)
            {
                if (enumerator.MoveNext())
                {
                    ProxyModelMetadata element = enumerator.Current;
                    yield return element;

                    stack.Push(enumerator);
                    enumerator = element.Properties.GetEnumerator();
                }
                else if (stack.Count > 0)
                {
                    enumerator.Dispose();
                    enumerator = stack.Pop();
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}