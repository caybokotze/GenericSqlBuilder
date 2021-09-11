using System.Text.RegularExpressions;

namespace GenericSqlBuilder.Tests
{
    public static class Utils
    {
        public static string RemoveDoubleWhiteSpace(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }
    }
}