namespace Yaginx.Infrastructure
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string str) =>
            string.IsNullOrEmpty(str) || str.Length < 2
            ? str.ToLowerInvariant()
            : char.ToLowerInvariant(str[0]) + str.Substring(1);
        public static string ToPascalCase(this string str) =>
            string.IsNullOrEmpty(str) || str.Length < 2
            ? str.ToUpperInvariant()
            : char.ToUpperInvariant(str[0]) + str.Substring(1);
    }
}
