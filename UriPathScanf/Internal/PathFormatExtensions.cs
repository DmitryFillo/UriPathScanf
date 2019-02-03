using System;
using System.Collections.Generic;
using System.Text;

namespace UriPathScanf.Internal
{
    internal static class PathFormatExtensions
    {
        public static string ToPlaceholderVariable(this string str) => '{' + str + '}';
        public static bool IsPlaceholder(this string str) => str == "{}";
        public static bool IsPlaceholderVariable(this string str) => str[0] == '{' && str[str.Length - 1] == '}';
        public static string GetNameOfPlaceholderVariable(this string str) => str.Trim('{', '}');
    }
}
