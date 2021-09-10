using PeanutButter.Utils;

namespace GenericSqlBuilder
{
    public static class CaseConverter
    {
        public static string ConvertCase(this string str, Casing casing)
        {
            return casing switch
            {
                Casing.UpperCase => str.ToUpperInvariant(),
                Casing.KebabCase => str.ToKebabCase(),
                Casing.PascalCase => str.ToPascalCase(),
                Casing.SnakeCase => str.ToSnakeCase(),
                Casing.LowerCase => str.ToLowerInvariant(),
                Casing.CamelCase => str.ToCamelCase(),
                _ => str
            };
        }
    }
}