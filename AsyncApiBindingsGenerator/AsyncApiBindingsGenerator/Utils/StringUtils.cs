using System;

namespace AsyncApiBindingsGenerator.Utils
{
    internal static class StringUtils
    {
        public static string ToPascalCase(string input)
        {
            return $"{char.ToUpper(input[0])}{input.Substring(1)}";
        }

        public static string GetRequestType(string restMethod)
        {
            switch (restMethod)
            {
                case "get":
                    return "Query";
                case "delete":
                    return "Query";
                case "post":
                    return "Command";
                default:
                    throw new ArgumentException($"Rest method '{restMethod}' not supported.");
            }
        }
    }
}
