using System;
using System.Collections.Generic;
using PeanutButter.Utils;

namespace GenericSqlBuilder
{
    public class SelectStatement : Statement
    {
        private readonly SqlBuilderOptions _sqlBuilderOptions;
        private bool _appendSelect;

        public SelectStatement(List<string> statements, SqlBuilderOptions sqlBuilderOptions) : base(sqlBuilderOptions)
        {
            foreach (var item in statements)
            {
                AddStatement(item);
            }

            _sqlBuilderOptions = sqlBuilderOptions;
            
            GenerateSqlStatement();
        }

        public SelectStatement(string sql, SqlBuilderOptions sqlBuilderOptions) : base(sqlBuilderOptions)
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
                    switch (_sqlBuilderOptions.IsDistinctSelect)
                    {
                        case true:
                            AddStatement("SELECT DISTINCT ");
                            break;
                        case false:
                            AddStatement("SELECT ");
                            break;
                    }

                    AddStatement(sql);
                    break;
            }
        }

        public string LastInserted(Version version)
        {
            switch (version)
            {
                case Version.MsSql:
                    AddStatement("SCOPE_IDENTITY() ");
                    break;
                case Version.MySql:
                    AddStatement("LAST_INSERT_ID() ");
                    break;
                default: 
                    AddStatement("LAST_INSERT_ID() ");
                    break;
            }
            
            return GenerateSqlStatement();
        }

        public SelectStatement AppendSelect(string sql)
        {
            _appendSelect = true;
            AddStatement(", " + sql + " ", true);
            return this;
        }

        public SelectStatement AppendSelect<T>() where T : class, new()
        {
            _appendSelect = true;
            CreateSelectStatement<T>();
            return this;
        }

        public SelectStatement AppendSelect<T>(Action<ISelectOptions> options) where T : class, new()
        {
            _appendSelect = true;
            _sqlBuilderOptions.ClearAll();
            options(_sqlBuilderOptions);
            return AppendSelect<T>();
        }
        
        public SelectStatement From(string table)
        {
            AddStatement($"FROM {table} ");
            return this;
        }
        
        public SelectStatement Where(string clause)
        {
            AddStatement($"WHERE {clause} ");
            return this;
        }

        public SelectStatement And(string clause)
        {
            AddStatement($"AND {clause} ");
            return this;
        }

        public SelectStatement And()
        {
            AddStatement($"AND ");
            return this;
        }

        public SelectStatement Equals(string clause)
        {
            AddStatement($"= {clause} ");
            return this;
        }

        public SelectStatement Like(string clause)
        {
            AddStatement($"LIKE {clause} ");
            return this;
        }

        public SelectStatement Is()
        {
            AddStatement("IS ");
            return this;
        }

        public JoinStatement LeftJoin(string table)
        {
            AddStatement($"LEFT JOIN {table} ");
            return new JoinStatement(GetStatements(), _sqlBuilderOptions);
        }

        public JoinStatement RightJoin(string table)
        {
            AddStatement($"RIGHT JOIN {table} ");
            return new JoinStatement(GetStatements(), _sqlBuilderOptions);
        }

        public JoinStatement InnerJoin(string table)
        {
            AddStatement($"INNER JOIN {table} ");
            return new JoinStatement(GetStatements(), _sqlBuilderOptions);
        }
        
        public void CreateSelectStatement<T>() where T : class, new()
        {
            if (_appendSelect)
            {
                AddStatement(", ", true);
            }
            
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