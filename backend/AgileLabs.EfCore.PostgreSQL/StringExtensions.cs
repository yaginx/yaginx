namespace AgileLabs.EfCore.PostgreSQL
{
    public static class StringExtensions
    {
        private const string EndfixEntity = "Entity";
        public static string FormatToTableName(this string value)
        {
            if (value.EndsWith(EndfixEntity))
            {
                value = value.Substring(0, value.Length - EndfixEntity.Length);
            }
            return Regex.Replace(value, @"([a-z])([A-Z])", "$1_$2").ToLower();
        }
    }
}
