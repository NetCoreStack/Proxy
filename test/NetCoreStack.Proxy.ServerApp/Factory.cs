using System.Net.Http;

namespace NetCoreStack.Proxy.ServerApp
{
    public static class Factory
    {
        public static HttpClient Client { get; }

        static Factory()
        {
            Client = new HttpClient();
        }
    }
}
