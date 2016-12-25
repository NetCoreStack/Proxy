namespace NetCoreStack.Proxy.Internal
{
    public class Constants
    {
        public static string Prefix = "NetCore";

        public const string ProxySettings = "ProxySettings";
        public const string ContentTypeJsonWithEncoding = "application/json; charset=utf-8";
        public const string CollectionResultItemKey = "CollectionResultItemKey";

        public readonly static string ClientUserAgentHeader = "User-Agent";
        public readonly static string MetadataHeader = $"X-{Prefix}-Metadata";
        public readonly static string MetadataInnerHeader = $"X-{Prefix}-MetadataInner";
    }
}
