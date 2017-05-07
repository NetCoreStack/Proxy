using System;
using System.Text.RegularExpressions;

namespace NetCoreStack.Proxy.Extensions
{
    internal static class StringExtensions
    {
        const string rawControllerDefinition = "[Controller]";
        const string regexForApi = "^I(.*)Api$";

        private static string regexControllerDefinition => rawControllerDefinition.Replace("[", "\\[")
            .Replace("]", "\\]");

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string GetApiRootPath(this string name, string template)
        {
            var defaultRootPath = $"api/{rawControllerDefinition}";
            if (template.Equals(defaultRootPath, StringComparison.OrdinalIgnoreCase))
            {
                var apiPath = Regex.Match(name, regexForApi);
                if (!apiPath.Success)
                    throw new InvalidOperationException($"API - Proxy name format is invalid." +
                        $"The valid format for API - Proxy Regex is: \"{regexForApi}\"!");

                var rootName = apiPath.Groups[1].Value;
                return Regex.Replace(template, regexControllerDefinition, rootName, RegexOptions.IgnoreCase);
            }

            return template;
        }

        public static bool IsJson(this string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }
    }
}
