namespace NetCoreStack.Proxy
{
    public interface IModelSerializer
    {
        string Serialize(object value);
    }
}