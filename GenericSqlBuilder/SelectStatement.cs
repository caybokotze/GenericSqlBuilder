using System;
using PeanutButter.Utils;

namespace GenericSqlBuilder
{
    public class SelectStatement : Statement
    {
        private SqlBuilderOptions _sqlBuilderOptions;
        private bool _appendSelect;

        public SelectStatement(string sql, SqlBuilderOptions sqlBuilderOptions = null) : base(sqlBuilderOptions)
        {
            _appendSelect = false;
            _sqlBuilderOptions = sqlBuilderOptions 
                                   ?? new SqlBuilderOptions();
            
            switch (_sqlBuilderOptions.IsAppendStatement)
            {
                case true:
                    AddStatement(sql);
                    AddStatement("SELECT ");
                    break;
                case false:
                    AddStatement("SELECT ");
                    AddStatement(sql);
                    break;
            }
        }

        public string LastInserted(Version version)
        {
            AddStatement("LAST_INSERT_ID() ");
            return GenerateSqlStatement();
        }

        public SelectStatement AppendSelect(string sql)
        {
            _appendSelect = true;
            return this;
        }

        public SelectStatement AppendSelect<T>() where T : class, new()
        {
            _appendSelect = true;
            CreateSelectStatement<T>();
            return this;
        }

        public SelectStatement AppendSelect<T>(Action<Options> options) where T : class, new()
        {
            _appendSelect = true;
            var selectStatementOptions = new SqlBuilderOptions();
            options(_sqlBuilderOptions);
            // _sqlBuilderOptions = selectStatementOptions;
            return AppendSelect<T>();
        }
        
        public void CreateSelectStatement<T>()
        {
            if (_appendSelect)
            {
                AddStatement(", ", true);
            }
            
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

            _appendSelect = false;
        }
    }
}