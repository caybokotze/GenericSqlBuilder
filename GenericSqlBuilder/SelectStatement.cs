using PeanutButter.Utils;

namespace GenericSqlBuilder
{
    public class SelectStatement : Statement
    {
        private readonly SqlStatementOptions _sqlStatementOptions;

        public SelectStatement(string properties, SqlStatementOptions sqlStatementOptions = null)
        {
            _sqlStatementOptions = sqlStatementOptions;
            AddStatement("SELECT ");
            AddStatement(properties);
        }

        public string LastInserted(Version version)
        {
            AddStatement("LAST_INSERT_ID() ");
            return GenerateSqlStatement();
        }
        
        public void CreateSelectStatement<T>()
        {
            var remove = _sqlStatementOptions.GetIgnoredProperties();
            var dataTable = GenericPropertyBuilder<T>.GetGenericProperties();
            
            if (remove != null && remove.Count > 0)
            {
                foreach (var item in remove)
                {
                    dataTable.Columns.Remove(item);
                }
            }

            var variableArray = new string[dataTable.Columns.Count];
            for (var i = 0; i < dataTable.Columns.Count; i ++)
            {
                variableArray[i] = dataTable.Columns[i].ColumnName
                    .ConvertCase(_sqlStatementOptions.GetCase());
            }

            AddStatement(string.Join(", ", variableArray) + " ");
        }
    }

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