namespace GenericSqlBuilder
{
    public class UpdateStatement : Statement
    {
        private readonly string _sql;
        private readonly SqlBuilderOptions _sqlBuilderOptions;

        public UpdateStatement(string sql, SqlBuilderOptions sqlBuilderOptions) : base(sqlBuilderOptions)
        {
            _sql = sql;
            _sqlBuilderOptions = sqlBuilderOptions;
            
            switch (_sqlBuilderOptions.IsAppendStatement)
            {
                case true:
                    AddStatement(sql);
                    AddStatement("UPDATE ");
                    break;
                case false:
                    AddStatement("UPDATE ");
                    AddStatement(sql);
                    break;
            }
        }

        public UpdateStatement Set(string clause)
        {
            AddStatement("SET ");
            AddStatement($"{clause} ");
            return this;
        }

        public UpdateStatement Where(string clause)
        {
            AddStatement("WHERE ");
            AddStatement($"{clause} ");
            return this;
        }
    }

    public class UpdateStatement<T> : Statement where T : class, new()
    {
        private readonly string _sql;
        private readonly SqlBuilderOptions _sqlBuilderOptions;

        public UpdateStatement(string sql, SqlBuilderOptions sqlBuilderOptions) : base(sqlBuilderOptions)
        {
            _sql = sql;
            _sqlBuilderOptions = sqlBuilderOptions;

            switch (_sqlBuilderOptions.IsAppendStatement)
            {
                case true:
                    AddStatement(sql);
                    AddStatement("UPDATE ");
                    break;
                case false:
                    AddStatement("UPDATE ");
                    AddStatement(sql);
                    break;
            }
        }
        
        public UpdateStatement<T> Set()
        {
            AddStatement("SET ");
            GenerateUpdateProperties();
            return this;
        }

        public UpdateStatement<T> Where(string clause)
        {
            AddStatement("WHERE ");
            AddStatement($"{clause} ");
            return this;
        }

        private void GenerateUpdateProperties()
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
                var attribute = dataTable.Columns[i].ColumnName;
                var property = "";
                
                switch (_sqlBuilderOptions.GetSqlVersion())
                {
                    case Version.MsSql:
                        property = $"[{variableArray[i]}]";
                        break;
                    case Version.MySql:
                        property = $"`{variableArray[i]}`";
                        break;
                    case Version.Generic:
                        property = variableArray[i];
                        break;
                }

                variableArray[i] = $"{property} = @{attribute}";
            }
            
            AddStatement($"{string.Join(", ", variableArray)}" + " ");
        }
    }
}