using System;
using System.Globalization;

namespace NetCoreStack.Proxy.Internal
{
    public interface ICultureFactory
    {
        CultureInfo Invoke();
    }
}
