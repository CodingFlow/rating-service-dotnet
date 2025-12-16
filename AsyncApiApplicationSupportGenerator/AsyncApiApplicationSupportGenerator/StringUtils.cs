namespace AsyncApiApplicationSupportGenerator
{
    internal static class StringUtils
    {
        public static string ToPascalCase(string input)
        {
            return $"{char.ToUpper(input[0])}{input.Substring(1)}";
        }
    }
}