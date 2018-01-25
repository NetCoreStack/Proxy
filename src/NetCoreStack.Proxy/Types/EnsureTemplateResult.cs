namespace NetCoreStack.Proxy
{
    public struct EnsureTemplateResult
    {
        public static EnsureTemplateResult Completed(int parameterOffset, bool ignorePrefix)
        {
            return new EnsureTemplateResult(parameterOffset: parameterOffset, completed: true, ignorePrefix: ignorePrefix);
        }

        public static EnsureTemplateResult Continue(int parameterOffset, bool ignorePrefix)
        {
            return new EnsureTemplateResult(parameterOffset: parameterOffset, completed: false, ignorePrefix: ignorePrefix);
        }

        public bool BindingCompleted { get; }
        public bool IgnoreModelPrefix { get; }
        public int ParameterOffset { get; }

        private EnsureTemplateResult(int parameterOffset, bool completed, bool ignorePrefix)
        {
            ParameterOffset = parameterOffset;
            BindingCompleted = completed;
            IgnoreModelPrefix = ignorePrefix;
        }
    }
}
