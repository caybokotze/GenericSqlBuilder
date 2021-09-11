using System.Collections.Generic;
using PeanutButter.Utils;

namespace GenericSqlBuilder
{
    public class InsertStatement : Statement
    {
        private readonly string _sql;
        private readonly SqlBuilderOptions _sqlBuilderOptions;

        public InsertStatement(string sql, SqlBuilderOptions sqlBuilderOptions) : base(sqlBuilderOptions)
        {
            _sql = sql;
            _sqlBuilderOptions = sqlBuilderOptions;

            AddStatement("INSERT INTO ");
            AddStatement(sql);
        }

        public InsertStatement(List<string> statements, SqlBuilderOptions sqlBuilderOptions) : base(statements)
        {
            _sqlBuilderOptions = sqlBuilderOptions;
            foreach (var item in statements)
            {
                AddStatement(item);
            }
        }

        public InsertStatement Values(string values)
        {
            AddStatement(values);
            return this;
        }

        public void GenerateInsertProperties<T>()
        {
            var remove = _sqlBuilderOptions.GetRemovedSelectProperties();
            var dataTable = GenericPropertyBuilder<T>.GetGenericProperties();

            foreach (var item in _sqlBuilderOptions.GetAddedSelectProperties())
            {
                dataTable.Columns.Add(item);
            }
            
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
}