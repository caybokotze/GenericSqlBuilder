﻿namespace GenericSqlBuilder
{
    public class InsertStatement : Statement
    {
        private readonly string _sql;
        private readonly SqlBuilderOptions _sqlBuilderOptions;

        public InsertStatement(string sql, SqlBuilderOptions sqlBuilderOptions, bool addIgnore = false) : base(sqlBuilderOptions)
        {
            _sql = sql;
            _sqlBuilderOptions = sqlBuilderOptions;

            switch (addIgnore)
            {
                case false:
                    AddStatement("INSERT INTO ");
                    break;
                case true:
                    AddStatement("INSERT IGNORE INTO ");
                    break;
            }
            
            AddStatement(sql);
        }

        public InsertStatement Values(string values)
        {
            AddStatement($"VALUES ({values}) ");
            return this;
        }
    }

    public class InsertStatement<T> : Statement where T : class, new() 
    {
        private readonly string _sql;
        private readonly SqlBuilderOptions _sqlBuilderOptions;

        public InsertStatement(string sql, SqlBuilderOptions sqlBuilderOptions, bool addIgnore = false) : base(sqlBuilderOptions)
        {
            _sql = sql;
            _sqlBuilderOptions = sqlBuilderOptions;

            switch (addIgnore)
            {
                case false:
                    AddStatement("INSERT INTO ");
                    break;
                case true:
                    AddStatement("INSERT IGNORE INTO ");
                    break;
            }

            AddStatement(sql);
        }

        public InsertStatement<T> Values()
        {
            AddStatement("VALUES ");
            GenerateInsertAttributes();
            return this;
        }

        private void GenerateInsertAttributes()
        {
            var remove = _sqlBuilderOptions.GetRemovedProperties();
            var dataTable = GenericPropertyBuilder<T>.GetPropertiesFromType();

            foreach (var item in _sqlBuilderOptions.GetAddedProperties())
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
                variableArray[i] = dataTable.Columns[i].ColumnName;

                variableArray[i] = $"@{variableArray[i]}";
            }
            
            AddStatement($"({string.Join(", ", variableArray)})" + " ");
        }
        
        public void GenerateInsertProperties()
        {
            var remove = _sqlBuilderOptions.GetRemovedProperties();
            var dataTable = GenericPropertyBuilder<T>.GetPropertiesFromType();

            foreach (var item in _sqlBuilderOptions.GetAddedProperties())
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
            
            AddStatement($"({string.Join(", ", variableArray)})" + " ");
        }
    }
}