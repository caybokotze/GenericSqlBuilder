using System;
using PeanutButter.Utils;

namespace GenericSqlBuilder
{
    public class SelectStatement : Statement
    {
        private readonly SqlBuilderOptions _sqlBuilderOptions;

        public SelectStatement(string sql, SqlBuilderOptions sqlBuilderOptions = null) : base(sqlBuilderOptions)
        {
            _sqlBuilderOptions = sqlBuilderOptions 
                                   ?? new SqlBuilderOptions();
            
            if (sqlBuilderOptions != null && !sqlBuilderOptions.IsAppendStatement())
            {
                AddStatement("SELECT ");
                AddStatement(sql);
            }
            if (sqlBuilderOptions != null && sqlBuilderOptions.IsAppendStatement())
            {
                AddStatement(sql);
            }

            _sqlBuilderOptions.SetAppendStatement(false);
        }

        public string LastInserted(Version version)
        {
            // if (!_sqlBuilderOptions.IsAppendStatement() 
            //     && !_sqlBuilderOptions.GetAlias().IsNullOrEmpty())
            // {
            //     AddStatement("SELECT ");
            // }
            AddStatement("LAST_INSERT_ID() ");
            return GenerateSqlStatement();
        }
        
        public void CreateSelectStatement<T>()
        {
            var remove = _sqlBuilderOptions.GetIgnoredProperties();
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
                    .ConvertCase(_sqlBuilderOptions.GetCase());

                if (!_sqlBuilderOptions.GetAlias().IsNullOrEmpty())
                {
                    variableArray[i] = $"{_sqlBuilderOptions.GetAlias()}.{variableArray[i]}";
                }

                switch (_sqlBuilderOptions.GetSqlVersion())
                {
                    case Version.MsSql:
                        variableArray[i] = $"[{variableArray[i]}]";
                        break;
                    case Version.MySql:
                        variableArray[i] = $"`{variableArray[i]}`";
                        break;
                }
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