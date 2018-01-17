namespace NetCoreStack.Proxy
{
    public interface IContentModelBinder
    {
        void BindContent(ContentModelBindingContext bindingContext);
    }
}