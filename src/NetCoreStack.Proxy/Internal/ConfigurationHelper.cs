using NetCoreStack.Proxy.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCoreStack.Proxy.Internal
{
    public static class ConfigurationHelper
    {
        public static string GetConfigurationContextDetail(this object configuration, object properties)
        {
            StringBuilder sb = new StringBuilder();
            var dictionary = properties.ToDictionary();
            foreach (KeyValuePair<string, object> entry in dictionary)
            {
                sb.AppendLine(entry.Value?.ToString());
            }

            return sb.ToString();
        }
    }
}
