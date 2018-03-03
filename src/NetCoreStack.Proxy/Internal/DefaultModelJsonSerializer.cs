using Newtonsoft.Json;

namespace NetCoreStack.Proxy.Internal
{
    public class DefaultModelJsonSerializer : IModelJsonSerializer
    {
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}