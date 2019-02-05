namespace UriPathScanf
{
    internal static class UriPathFormatExtensions
    {
        public static string ToPlaceholderVariable(this string str) => '{' + str + '}';
        public static bool IsUnboundPlaceholder(this string str) => str == "{}";
        public static bool IsPlaceholderVariable(this string str) => str[0] == '{' && str[str.Length - 1] == '}';
        public static string GetNameOfPlaceholderVariable(this string str) => str.Trim('{', '}');
    }
}
