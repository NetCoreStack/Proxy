using System;
using System.Globalization;

namespace NetCoreStack.Proxy.Internal
{
    public class DefaultCultureFactory : ICultureFactory
    {
        private readonly Func<CultureInfo> _func;

        public DefaultCultureFactory(Func<CultureInfo> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public CultureInfo Invoke()
        {
            return _func();
        }
    }
}
