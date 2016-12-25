using System;
using System.Text.RegularExpressions;

namespace NetCoreStack.Proxy.Extensions
{
    public static class StringExtensions
    {
        const string rawControllerDefinition = "[Controller]";
        const string regexForApi = "^I(.*)Api$";
        const string ApiRootPath = "api/";

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string GetApiRawName(this string name, string path)
        {
            var apiPath = Regex.Match(name, regexForApi);
            if (!apiPath.Success)
                throw new InvalidOperationException($"API - Proxy name format is invalid." +
                    $"The valid format for API - Proxy Regex is: \"{regexForApi}\"!");

            return ApiRootPath + apiPath.Groups[1].Value;
        }

        public static bool IsJson(this string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }
    }
}
